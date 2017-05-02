using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Sts.Model.V20150401;
using cloudscribe.Core.Identity;
using cloudscribe.Core.Models;
using cloudscribe.Core.Web;
using cloudscribe.Web.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneEchan.Server.Common.Extensions;
using OneEchan.Server.Controllers.Interface;
using OneEchan.Server.Data;
using OneEchan.Server.Models;
using OneEchan.Server.Models.Aliyun;
using OneEchan.Server.Models.PostViewModel;
using OneEchan.Server.Models.VideoViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Acs.Mts.Model.V20140618;
using cloudscribe.Core.Web.Controllers;

namespace OneEchan.Server.Controllers
{
    public class VideoController : Controller, IPostController
    {
        private readonly ApplicationDbContext _context;
        private readonly SiteUserManager<SiteUser> _userManager;
        private readonly IStringLocalizer<CloudscribeCore> _sr;
        private readonly AliyunOptions _aliyunOptions;
        private readonly TranscodeDbContext _transcodeContext;

        public VideoController(ApplicationDbContext context, TranscodeDbContext transcodeContext, SiteUserManager<SiteUser> userManager, IStringLocalizer<CloudscribeCore> localizer, IOptions<AliyunOptions> aliyunOptions)
        {
            _context = context;
            _userManager = userManager;
            _sr = localizer;
            _aliyunOptions = aliyunOptions.Value;
            _transcodeContext = transcodeContext;
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCallback(UploadCallbackModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToLocal(nameof(Create));
            }
            var media = new DefaultAcsClient(DefaultProfile.GetProfile(_aliyunOptions.RegionId,
                    _aliyunOptions.MtsAccount.AccessKeyId,
                    _aliyunOptions.MtsAccount.AccessKeySecret))
                .GetAcsResponse(new ListMediaWorkflowExecutionsRequest
                {
                    MediaWorkflowId = _aliyunOptions.MediaWorkflowId,
                    InputFileURL = $"http://{model.Bucket}.{new Uri(model.Endpoint).Host}/{model.ObjectName}"
                }).MediaWorkflowExecutionList.FirstOrDefault();
            var video = new Video
            {
                Title = Path.GetFileNameWithoutExtension(model.FileName),
                UploaderId = new Guid(User.GetUserId()),
                Language = 0,
                Ip = Request.HttpContext.GetIpV4Address(),
                PostState = Post.State.Editing,
                Category = _context.Category.FirstOrDefault()
            };
            await _context.Video.AddAsync(video);
            await _context.SaveChangesAsync();
            await _transcodeContext.Transcode.AddAsync(new TranscodeModel
            {
                MediaId = media.MediaId,
                RunId = media.RunId,
                VideoId = video.Id,
            });
            await _transcodeContext.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = video.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public AliyunStsSignatureModel GetSignature()
        {
            return new AliyunStsSignatureModel(new DefaultAcsClient(DefaultProfile.GetProfile(_aliyunOptions.RegionId,
                    _aliyunOptions.UploadAccount.AccessKeyId, _aliyunOptions.UploadAccount.AccessKeySecret))
                .GetAcsResponse(new AssumeRoleRequest
                {
                    Method = MethodType.POST,
                    Protocol = ProtocolType.HTTPS,
                    DurationSeconds = 3600L,
                    RoleArn = _aliyunOptions.RoleArn,
                    RoleSessionName = "test"
                })
                .Credentials, _aliyunOptions);
        }

        //[HttpPost]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadComplete(UploadCallbackModel model)
        //{

        //}

        //[Authorize]
        //public AliyunSignatureModel GetSignature()
        //{
        //    var dir = $"{User.GetUserId().ToBase64()}/";
        //    var callback = new AliyunSendCallbackModel(Url.Action(nameof(UploadCallback), nameof(VideoController).Replace("Controller", ""), null, Request.Scheme)).ToString();
        //    var expire = DateTime.Now.AddSeconds(30).FormatIso8601Date();
        //    var policy = new AliyunPolicyModel(dir, callback).ToString();
        //    var signature = "";
        //    using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_aliyunOptions.Key)))
        //        signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(policy)));
        //    return new AliyunSignatureModel
        //    {
        //        AccessId = _aliyunOptions.Id,
        //        Host = _aliyunOptions.Host,
        //        Policy = policy,
        //        Expire = expire,
        //        Callback = callback,
        //        Dir = dir,
        //        Signature = signature,
        //        UserId = User.GetUserId().ToBase64()
        //    };
        //}

        //[HttpPost]
        //public async Task<string> UploadCallback(AliyunCallbackModel model)
        //{
        //    var pubKeyAddr = Request.Headers["x-oss-pub-key-url"].FirstOrDefault()?.FromBase64();
        //    if (string.IsNullOrEmpty(pubKeyAddr) || !pubKeyAddr.StartsWith("http://gosspublic.alicdn.com/") || !pubKeyAddr.StartsWith("https://gosspublic.alicdn.com/"))
        //    {
        //        return "Hey!";//Fake public key
        //    }
        //    string pubKey;
        //    using (var client = new HttpClient())
        //        pubKey = (await client.GetStringAsync(pubKeyAddr));//.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");
        //    var url = Request.GetCurrentFullUrlWithQueryString();
        //    var authorization = Convert.FromBase64String(Request.Headers["Authorization"].FirstOrDefault());
        //    var auth = "";
        //    using (var reader = new StreamReader(Request.Body))
        //        auth = await reader.ReadToEndAsync();
        //    auth = $"{url}\n{auth}";

        //    if (VerifySignature(pubKey, auth, authorization))
        //    {
        //        //TODO: insert the data to transcode database


        //        var retbody = "{\"Status\":\"OK\"}";
        //        Response.Headers["Content-Type"] = "application/json";
        //        Response.StatusCode = 200;
        //        return retbody;
        //    }
        //    else
        //    {
        //        var retbody = "{\"Status\":\"verdify not ok\"}";
        //        Response.Headers["Content-Type"] = "application/json";
        //        Response.StatusCode = 400;
        //        return retbody;
        //    }
        //}


        //private bool VerifySignature(string pubKey, string data, byte[] signature)
        //{
        //    using (var cert = new X509Certificate2(Encoding.UTF8.GetBytes(pubKey)))
        //    using (var rsa = cert.GetRSAPublicKey())
        //    {
        //        var dataBytes = Encoding.UTF8.GetBytes(data);
        //        return rsa.VerifyData(dataBytes, signature, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);//TODO: not sure if it works
        //    }
        //}


        public async Task<IActionResult> Details(int id)
        {
            var video = await _context.Video
                .Include(v => v.Attitude)
                .Include(v => v.Comment)
                .Include(v => v.VideoUrl)
                .Include(v => v.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (video?.PostState != Post.State.Published)
                return NotFound();
            var uploader = await _userManager.FindByIdAsync(video.UploaderId.ToString());
            var likeCount = video.Attitude.Where(item => item.AttitudeType == Attitude.Type.Like).LongCount();
            var total = video.Attitude.LongCount();
            return View(new PostDetailViewModel
            {
                Post = video,
                Uploader = uploader,
                LikeCount = likeCount,
                TotalAttitudeCount = total
            });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var video = await _context.Video
                .Include(v => v.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (video == null)
                return NotFound();
            if (video.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }
            var categories = await _context.Category.ToArrayAsync();
            ViewBag.Categories = categories;
            return View(video);
            //return View(new VideoEditViewModel
            //{
            //    Categories = categories,
            //    Video = video
            //});
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VideoEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            var video = await _context.Video
                .Include(v => v.Category)
                .FirstOrDefaultAsync(item => item.Id == model.Id);
            if (video == null)
                return NotFound();
            if (video.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            video.Title = model.Title;
            video.Description = model.Description;
            video.AllowComment = model.AllowComment;
            video.CategoryId = model.CategoryId;
            if (video.PostState == Post.State.Editing)
            {
                video.PostState = video.VideoState == Video.VideoStates.Complete ? Post.State.Published : Post.State.Publishing;
            }
            _context.Video.Update(video);
            await _context.SaveChangesAsync();
            this.AlertSuccess(_sr["Your video has been changed."]);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var video = await _context.Video.FindAsync(id);
            if (video == null)
                return NotFound();
            if (video.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }
            _context.Video.Remove(video);
            await _context.SaveChangesAsync();
            this.AlertSuccess(_sr["Your video has been removed"]);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }


}
