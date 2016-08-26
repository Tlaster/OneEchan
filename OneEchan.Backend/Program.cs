using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using OneEchan.Backend.Models;
using OneEchan.Backend.QCloud.Api;
using OneEchan.Core.Models;

namespace OneEchan.Backend
{
    public class CheckModel
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string SetName { get; set; }
        public string ZhTW { get; set; }

        public override string ToString() 
            => $"{Name} {SetName}";
    }
    public class Program
    {
        private static VideoCloud Cloud { get; set; } 
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private static string DownloadFolder => Configuration[nameof(DownloadFolder)];
        private static int AppID => int.Parse(Configuration[nameof(AppID)]);
        private static string SecretID => Configuration[nameof(SecretID)];
        private static string SecretKey => Configuration[nameof(SecretKey)];
        private static string AccessToken => Configuration[nameof(AccessToken)];
        private static string RegexPattern => Configuration[nameof(RegexPattern)];
        private static string BucketName => Configuration[nameof(BucketName)];
        private static bool ShareToWeibo => bool.Parse(Configuration[nameof(ShareToWeibo)]);
        private static string EnSite => Configuration[nameof(EnSite)];
        private static string ZhSite => Configuration[nameof(ZhSite)];
        private static string RuSite => Configuration[nameof(RuSite)];
        private static IConfigurationRoot Configuration { get; set; }
        private static List<CheckModel> CheckList { get; } = new List<CheckModel>();
        private static List<CheckModel> WeiboList { get; } = new List<CheckModel>();

        public static void Main(string[] args)
        {
            //NOTICE: THIS BACKEND SCRIPT HAS NOT BEEN FULLY TESTED
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Cloud = new VideoCloud(AppID, SecretID, SecretKey);
            Task.Run(async () =>
            {
                OpenWeen.Core.Api.Entity.AccessToken = AccessToken;
                while (true)
                {
                    Logger.Info("Starting...");
                    await CheckQuality();
                    if (ShareToWeibo)
                        await CheckWeiboShare();
                    await UploadTask();
                    Logger.Info("Waiting...");
                    await Task.Delay(TimeSpan.FromMinutes(5d));
                } 
            });
            Console.WriteLine("Running...");
            do
            {
                while (!Console.KeyAvailable)
                {
                    switch (Console.ReadLine())
                    {
                        case "status":
                            Console.WriteLine($"{nameof(CheckList)} {CheckList.Count}");
                            CheckList.ForEach(item => Console.WriteLine(item));
                            Console.WriteLine($"{nameof(WeiboList)} {WeiboList.Count}");
                            WeiboList.ForEach(item => Console.WriteLine(item));
                            Console.WriteLine(File.ReadLines("log.txt").LastOrDefault());
                            break;
                        default:
                            break;
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private static async Task CheckWeiboShare()
        {
            Logger.Info("checking for weibo share");
            for (int i = 0; i < WeiboList.Count; i++)
            {
                var item = WeiboList[i];
                Logger.Info($"checking for {item} weibo");
                dynamic obj = await UnlimitedRetry(Cloud.GetFileStat(BucketName, $"/{item.Name}/{item.SetName}"));
                if (!string.IsNullOrEmpty(obj.data?.video_cover))
                {
                    Logger.Info($"sending weibo {item}");
                    try
                    {
                        var url = $"http://OneEchan.moe/Watch?id={item.ID}&set={double.Parse(item.SetName)}";
                        var shorturl = await OpenWeen.Core.Api.ShortUrl.Shorten(url);
                        using (var client = new HttpClient())
                        using (Stream stream = await client.GetStreamAsync(obj.data?.video_cover))
                        using (var memStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memStream);
                            await OpenWeen.Core.Api.Statuses.PostWeibo.PostWithPic($"{item.ZhTW} - {item.SetName} {shorturl}", memStream.ToArray());
                        }
                        WeiboList.RemoveAt(i);
                        Logger.Info($"weibo {item} sended");
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"can not share to weibo {e.Message}");
                    }
                }
            }
            Logger.Info("checking for weibo is done");
        }

        private static async Task CheckQuality()
        {
            Logger.Info("checking for quality");
            for (int i = 0; i < CheckList.Count; i++)
            {
                var item = CheckList[i];
                Logger.Info($"checking for {item} quality");
                if (CheckForVideoQuality(item.Name, item.SetName, await UnlimitedRetry(Cloud.GetFileStat(BucketName, $"/{item.Name}/{item.SetName}"))))
                {
                    CheckList.RemoveAt(i);
                    Logger.Info($"{item} quality is done, remove");
                }
            }
            Logger.Info("checking for quality is done");
        }

        private static async Task UploadTask()
        {
            var files = Directory.GetFiles(DownloadFolder);
            foreach (var item in files)
            {
                if (Path.GetExtension(item) != ".mp4") continue;
                var match = Regex.Match(item, RegexPattern);
                if (!match.Success) continue;
                var title = GetTitle(match);
                var setName = GetSetName(match);
                Logger.Info($"detecting file {title} {setName}");
                dynamic obj = await UnlimitedRetry(Cloud.GetFolderStat(BucketName, $"/{title}/"));
                if (obj != null && obj.code != 0)
                {
                    Logger.Info($"create {title} folder");
                    await Cloud.CreateFolder(BucketName, $"/{title}/");
                }
                obj = null;
                obj = await UnlimitedRetry(Cloud.GetFileStat(BucketName, $"/{title}/{setName}"));
                if (obj != null && obj.code != 0)
                {
                    Logger.Info($"uploading {title} {setName}...");
                    await Cloud.SliceUploadFile(BucketName, $"/{title}/{setName}", item);
                    await AddSet(item, title, setName);
                    Logger.Info($"upload {title} {setName} complete");
                }
                else if (obj.code == 0)
                {
                    await CheckForVideoFile(item, title, setName, obj);
                }
            }
        }

        private static async Task AddSet(string item, string title, string setName)
        {
            Logger.Info($"adding {title} {setName} into database");
            using (var context = new AnimateDatabaseContext())
            {
                if (context.AnimateList.Any(anime => anime.EnUs == title))
                {
                    Logger.Info("cannot find id, create new");
                    context.AnimateList.Add(new AnimateList { EnUs = title, Updated_At = DateTime.Now });
                }
                var id = context.AnimateList.FirstOrDefault(anime => anime.EnUs == title).Id;
                if (id != -1)
                {
                    Logger.Info($"checking for file data...");
                    dynamic obj = await UnlimitedRetry(Cloud.GetFileStat(BucketName, $"/{title}/{setName}"));
                    if (obj != null && obj.code == 0)
                    {
                        if (obj.data.access_url != null)
                        {
                            Logger.Info("adding set...");
                            context.SetDetail.Add(new SetDetail { Id = id, SetName = double.Parse(setName), FilePath = obj.data.access_url, ClickCount = 0, Created_At = DateTime.Now });
                            var animeItem = context.AnimateList.FirstOrDefault(anime => anime.Id == id);
                            animeItem.Updated_At = DateTime.Now;
                            animeItem = await GetAnimeTitle(title, animeItem);
                            context.Entry(animeItem).State = EntityState.Modified;
                            context.SaveChanges();
                            CheckList.Add(new CheckModel { ID = id, Name = title, SetName = setName, ZhTW = animeItem.ZhTw });
                            if (ShareToWeibo)
                                WeiboList.Add(new CheckModel { ID = id, Name = title, SetName = setName, ZhTW = animeItem.ZhTw });
                            Logger.Info("add set complete");
                        }
                    }
                }
            }
        }

        private static async Task<AnimateList> GetAnimeTitle(string title, AnimateList animeItem)
        {
            Logger.Info($"getting anime title for {title}");
            try
            {
                if (string.IsNullOrEmpty(animeItem.JaJp) || string.IsNullOrEmpty(animeItem.RuRu) || string.IsNullOrEmpty(animeItem.ZhTw))
                {
                    animeItem.JaJp = await GetLName(title, EnSite, (node) =>
                    {
                        return new
                        {
                            Name = node.TextContent.Trim(),
                            LName = node.NextSibling.NextSibling.FirstChild.TextContent.Trim(),
                        };
                    });
                    animeItem.ZhTw = await GetLName(animeItem.JaJp, ZhSite, (node) =>
                    {
                        return new
                        {
                            LName = node.TextContent.Trim(),
                            Name = node.NextSibling.NextSibling.FirstChild.TextContent.Trim(),
                        };
                    });
                    animeItem.RuRu = await GetLName(animeItem.JaJp, RuSite, (node) =>
                    {
                        return new
                        {
                            LName = node.TextContent.Trim(),
                            Name = node.NextSibling.NextSibling.FirstChild.TextContent.Trim(),
                        };
                    });
                }

            }
            catch (Exception e)
            {
                Logger.Warn($"can not get JName {e.Message}");
            }
            Logger.Info($"get anime name complete, {animeItem.JaJp} {animeItem.RuRu} {animeItem.ZhTw}");
            return animeItem;
        }

        private static async Task<string> GetLName(string title, string link, Func<IElement, dynamic> gen)
        {
            string str = "";
            using (var client = new HttpClient())
            {
                str = await client.GetStringAsync(link);
            }
            var doc = new HtmlParser().Parse(str);

            var items = from element in doc.All
                        where element?.Attributes["class"]?.Value == "name"
                        select gen(element);
            if (items.ToList().FirstOrDefault(a => a.Name == title) != null)
            {
                return items.ToList().FirstOrDefault(a => a.Name == title).LName;
            }
            return null;
        }

        private static async Task CheckForVideoFile(string item, string title, string setName, dynamic obj)
        {
            var file = new FileInfo(item);
            if (obj != null && obj.data?.filesize != file.Length)
            {
                Logger.Warn($"file {title} {setName} reuploading");
                await Cloud.DeleteFile(BucketName, $"/{title}/{setName}");
                await Cloud.SliceUploadFile(BucketName, $"/{title}/{setName}", item);
                Logger.Info($"reupload {title} {setName} complete");
            }
        }

        private static bool CheckForVideoQuality(string title, string setName, dynamic obj)
        {
            Logger.Info($"checking for {title} {setName} video quality");
            using (var context = new AnimateDatabaseContext())
            {
                var id = context.AnimateList.FirstOrDefault(anime => anime.EnUs == title).Id;
                if (id != -1)
                {
                    var set = context.SetDetail.FirstOrDefault(anime => anime.Id == id && anime.SetName == double.Parse(setName));
                    if (set != null)
                    {
                        if (string.IsNullOrEmpty(set.FileThumb) && !string.IsNullOrEmpty(obj.data?.video_cover))
                        {
                            Logger.Info("add file thumb");
                            set.FileThumb = obj.data.video_cover;
                        }
                        if (string.IsNullOrEmpty(set.LowQuality) && !string.IsNullOrEmpty(obj.data?.video_play_url?.f10))
                        {
                            Logger.Info("add LowQuality");
                            set.LowQuality = obj.data.video_play_url.f10;
                        }
                        if (string.IsNullOrEmpty(set.MediumQuality) && !string.IsNullOrEmpty(obj.data?.video_play_url?.f20))
                        {
                            Logger.Info("add MediumQuality");
                            set.MediumQuality = obj.data.video_play_url.f20;
                        }
                        if (string.IsNullOrEmpty(set.HighQuality) && !string.IsNullOrEmpty(obj.data?.video_play_url?.f30))
                        {
                            Logger.Info("add HighQuality");
                            set.HighQuality = obj.data.video_play_url.f30;
                        }
                        if (string.IsNullOrEmpty(set.OriginalQuality) && !string.IsNullOrEmpty(obj.data?.video_play_url?.f0))
                        {
                            Logger.Info("add OriginalQuality");
                            set.OriginalQuality = obj.data.video_play_url.f0;
                        }
                        context.Entry(set).State = EntityState.Modified;
                        context.SaveChanges();
                        if (!string.IsNullOrEmpty(set.FileThumb) && !string.IsNullOrEmpty(set.LowQuality) && !string.IsNullOrEmpty(set.MediumQuality) && !string.IsNullOrEmpty(set.HighQuality) && string.IsNullOrEmpty(set.OriginalQuality))
                            return true;
                    }
                }
            }
            return false;
        }

        private static async Task<object> UnlimitedRetry(Task<string> task)
        {
            while (true)
            {
                try
                {
                    return JsonConvert.DeserializeObject(await task);
                }
                catch
                {
                    Logger.Error($"failed,retry");
                    continue;
                }
            }
        }

        private static string GetSetName(Match match) 
            => match.Groups[2].Value.Trim();

        private static string GetTitle(Match match)
            => match.Groups[1].Value.Trim();
    }
}
