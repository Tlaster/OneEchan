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
using OneEchan.Server.Controllers.Interface;
using OneEchan.Server.Data;
using OneEchan.Server.Models;
using OneEchan.Server.Models.Aliyun;
using OneEchan.Server.Models.PostViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{
    public class VideoController : Controller, IPostController
    {
        private readonly ApplicationDbContext _context;
        private SiteUserManager<SiteUser> _userManager;
        private IStringLocalizer<CloudscribeCore> _sr;
        private IOptions<AliyunOptions> _aliyunOptions;

        public VideoController(ApplicationDbContext context, SiteUserManager<SiteUser> userManager, IStringLocalizer<CloudscribeCore> localizer, IOptions<AliyunOptions> aliyunOptions)
        {
            _context = context;
            _userManager = userManager;
            _sr = localizer;
            _aliyunOptions = aliyunOptions;
        }

        public IActionResult Create(int id)
        {
            throw new NotImplementedException();
        }

        public AliyunSignatureModel GetSignature()
        {
            var callbackStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new AliyunCallbackModel(Url.Action(nameof(UploadCallback))))));
            var expiration = (DateTime.Now + new TimeSpan(0, 0, 10)).ToString("o");
        }

        public IActionResult UploadCallback()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Details(int id)
        {
            var video = await _context.Video
                .Include(v => v.Attitude)
                .Include(v => v.Comment)
                .Include(v => v.VideoUrl)
                .Include(v => v.Category)
                .Include(v => v.Category.CategoryName)
                .FirstOrDefaultAsync(item => item.Id == id);
            var uploader = await _userManager.FindByIdAsync(video.UploaderId.ToString());
            var likeCount = video.Attitude.Where(item => item.AttitudeType == Models.Attitude.Type.Like).LongCount();
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
                .Include(v => v.Category.CategoryName)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (video.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }
            return View(video);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VideoEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            var video = await _context.Video
                .Include(v => v.Category)
                .Include(v => v.Category.CategoryName)
                .FirstOrDefaultAsync(item => item.Id == model.Id);
            if (video.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            video.Name = model.Name;
            video.Description = model.Description;
            video.AllowComment = model.AllowComment;
            video.Ip = Request.HttpContext.GetIpV4Address();
            _context.Video.Update(video);
            await _context.SaveChangesAsync();
            this.AlertSuccess(_sr["Your video has been changed."]);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }
    }
}
