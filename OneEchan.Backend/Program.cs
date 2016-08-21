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
using OneEchan.Backend.Models;
using OneEchan.Backend.QCloud.Api;
using OneEchan.Core.Models;

namespace OneEchan.Backend
{
    public class Program
    {
        private static VideoCloud _cloud;

        public static string DownloadFolder => Configuration[nameof(DownloadFolder)];
        public static int AppID => int.Parse(Configuration[nameof(AppID)]);
        public static string SecretID => Configuration[nameof(SecretID)];
        public static string SecretKey => Configuration[nameof(SecretKey)];
        public static string AccessToken => Configuration[nameof(AccessToken)];
        public static string RegexPattern => Configuration[nameof(RegexPattern)];
        public static string BucketName => Configuration[nameof(BucketName)];
        public static bool ShareToWeibo => bool.Parse(Configuration[nameof(ShareToWeibo)]);
        public static string EnSite => Configuration[nameof(EnSite)];
        public static string ZhSite => Configuration[nameof(ZhSite)];
        public static string RuSite => Configuration[nameof(RuSite)];

        public static IConfigurationRoot Configuration { get; private set; }
        
        public static void Main(string[] args)
        {
            //NOTICE: THIS BACKEND SCRIPT HAS NOT BEEN FULLY TESTED
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Task.Run(async () =>
            {
                _cloud = new VideoCloud(AppID, SecretID, SecretKey);
                OpenWeen.Core.Api.Entity.AccessToken = AccessToken;
                while (true)
                {
                    await UploadTask();
                }
            }).Wait();
        }

        private static async Task UploadTask()
        {
            var files = Directory.GetFiles(DownloadFolder);
            foreach (var item in files)
            {
                var match = Regex.Match(item, RegexPattern);
                if (!match.Success) continue;
                var title = GetTitle(match);
                var setName = GetSetName(match);
                if (File.Exists($"{DownloadFolder}{Path.GetFileNameWithoutExtension(item)}.complete"))
                {
                    if (Path.GetExtension(item) == ".mp4")
                    {
                        dynamic jobj = await UnlimitedRetry(_cloud.GetFileStat(BucketName, $"/{title}/{setName}"));
                        await CheckForVideoFile(item, title, setName, jobj);
                        CheckForVideoQuality(title, setName, jobj);
                    }
                    continue;
                }
                if (Path.GetExtension(item) != ".mp4") continue;
                dynamic obj = await UnlimitedRetry(_cloud.GetFolderStat(BucketName, $"/{title}/"));
                if (obj != null && obj.code != 0)
                {
                    Console.WriteLine($"create {title} folder");
                    await _cloud.CreateFolder(BucketName, $"/{title}/");
                }
                obj = null;
                obj = await UnlimitedRetry(_cloud.GetFileStat(BucketName, $"/{title}/{setName}"));
                if (obj != null && obj.code != 0)
                {
                    await _cloud.SliceUploadFile(BucketName, $"/{title}/{setName}", item);
                    await AddSet(item, title, setName);
                }
                File.Create($"{DownloadFolder}{Path.GetFileNameWithoutExtension(item)}.complete").Dispose();
                Console.WriteLine($"upload {title} {setName} complete");
            }
            Console.WriteLine("Waiting...");
            await Task.Delay(TimeSpan.FromMinutes(5d));
        }

        private static async Task AddSet(string item, string title, string setName)
        {
            using (var context = new AnimateDatabaseContext())
            {
                if (context.AnimateList.FirstOrDefault(anime => anime.EnUs == title) == null)
                    context.AnimateList.Add(new AnimateList { EnUs = title, Updated_At = DateTime.Now });
                var id = context.AnimateList.FirstOrDefault(anime => anime.EnUs == title).Id;
                if (id != -1)
                {
                    dynamic obj = await UnlimitedRetry(_cloud.GetFileStat(BucketName, $"/{title}/{setName}"));
                    if (obj != null && obj.code == 0)
                    {
                        if (obj.data.access_url != null)
                        {
                            context.SetDetail.Add(new SetDetail { Id = id, SetName = double.Parse(setName), FilePath = obj.data.access_url, ClickCount = 0, Created_At = DateTime.Now });
                            var animeItem = context.AnimateList.FirstOrDefault(anime => anime.Id == id);
                            animeItem.Updated_At = DateTime.Now;
                            animeItem = await GetAnimeTitle(title, animeItem);
                            context.Entry(animeItem).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        if (ShareToWeibo)
                        {
                            var cname = context.AnimateList.FirstOrDefault(anime => anime.Id == id).ZhTw;
                            await WeiboShare(item, setName, id, cname);
                        }
                    }
                }
            }
        }

        private static async Task WeiboShare(string item, string setName, int id, string cname)
        {
            try
            {
                if (!string.IsNullOrEmpty(cname))
                {
                    var url = $"http://OneEchan.moe/Watch?id={id}&set={double.Parse(setName)}";
                    var shorturl = await OpenWeen.Core.Api.ShortUrl.Shorten(url);
                    await OpenWeen.Core.Api.Statuses.PostWeibo.Post($"{cname} - {setName} {url}");
                    //.net core can not use the NReco.VideoConverter, please wait...
                    //using (var stream = new MemoryStream())
                    //{
                    //    (new NReco.VideoConverter.FFMpegConverter()).GetVideoThumbnail(item, stream, new Random().Next(10, 120));
                    //    await OpenWeen.Core.Api.Statuses.PostWeibo.PostWithPic($"{cname} - {setName} {url}", stream.ToArray());
                    //}
                }
            }
            catch
            {
            }
        }

        private static async Task<AnimateList> GetAnimeTitle(string title, AnimateList animeItem)
        {
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
                Console.WriteLine($"can not get JName {e.Message}");
            }
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
            if (obj != null && (obj.code != 0 || obj.data.filesize != file.Length))
            {
                Console.WriteLine($"file {title} {setName} reuploading");
                await _cloud.DeleteFile(BucketName, $"/{title}/{setName}");
                await _cloud.SliceUploadFile(BucketName, $"/{title}/{setName}", item);
                Console.WriteLine("reupload complete");
            }
        }

        private static void CheckForVideoQuality(string title, string setName, dynamic obj)
        {
            using (var context = new AnimateDatabaseContext())
            {
                var id = context.AnimateList.FirstOrDefault(anime => anime.EnUs == title).Id;
                if (id != -1)
                {
                    var set = context.SetDetail.FirstOrDefault(anime => anime.Id == id && anime.SetName == double.Parse(setName));
                    if (set != null)
                    {
                        if (string.IsNullOrEmpty(set.FileThumb) && !string.IsNullOrEmpty(obj.data.video_cover))
                            set.FileThumb = obj.data.video_cover;
                        if (string.IsNullOrEmpty(set.LowQuality) && !string.IsNullOrEmpty(obj.data.video_play_url.f10))
                            set.LowQuality = obj.data.video_play_url.f10;
                        if (string.IsNullOrEmpty(set.MediumQuality) && !string.IsNullOrEmpty(obj.data.video_play_url.f20))
                            set.MediumQuality = obj.data.video_play_url.f20;
                        if (string.IsNullOrEmpty(set.HighQuality) && !string.IsNullOrEmpty(obj.data.video_play_url.f30))
                            set.HighQuality = obj.data.video_play_url.f30;
                        if (string.IsNullOrEmpty(set.OriginalQuality) && !string.IsNullOrEmpty(obj.data.video_play_url.f0))
                            set.OriginalQuality = obj.data.video_play_url.f0;
                        context.Entry(set).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
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
                    Console.WriteLine($"failed,retry");
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
