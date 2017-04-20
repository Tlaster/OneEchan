using cloudscribe.Core.Identity;
using cloudscribe.Core.Models;
using cloudscribe.Core.Web;
using cloudscribe.Web.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OneEchan.Server.Controllers.Interface;
using OneEchan.Server.Data;
using OneEchan.Server.Models.PostViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{
    public class ArticleController : Controller, IPostController
    {
        private ApplicationDbContext _context;
        private SiteUserManager<SiteUser> _userManager;
        private IStringLocalizer<CloudscribeCore> _sr;

        public ArticleController(ApplicationDbContext context, SiteUserManager<SiteUser> userManager, IStringLocalizer<CloudscribeCore> localizer)
        {
            _context = context;
            _userManager = userManager;
            _sr = localizer;
        }

        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Details(int id)
        {
            var video = await _context.Article
                .Include(v => v.Attitude)
                .Include(v => v.Comment)
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
        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var article = await _context.Article
                .Include(v => v.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (article.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }
            return View(article);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            var article = await _context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(item => item.Id == model.Id);
            if (article.UploaderId.ToString() != User.GetUserId())
            {
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            article.Title = model.Title;
            article.Content = model.Content;
            article.AllowComment = model.AllowComment;
            article.Ip = Request.HttpContext.GetIpV4Address();
            _context.Article.Update(article);
            await _context.SaveChangesAsync();
            this.AlertSuccess(_sr["Your article has been changed."]);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        public Task<IActionResult> Remove(int id, string returnUrl = null)
        {
            throw new NotImplementedException();
        }
    }
}
