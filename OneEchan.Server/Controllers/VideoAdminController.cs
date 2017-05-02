using cloudscribe.Core.Web;
using cloudscribe.Web.Common.Extensions;
using cloudscribe.Web.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OneEchan.Server.Data;
using OneEchan.Server.Models;
using OneEchan.Server.Models.PostViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class VideoAdminController : Controller
    {
        private const int PAGE_SIZE = 20;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<CloudscribeCore> _sr;

        public VideoAdminController(ApplicationDbContext context, IStringLocalizer<CloudscribeCore> localizer)
        {
            _context = context;
            _sr = localizer;
        }

        public IActionResult Index(int page = 0, string search = "")
        {
            var video = _context.Video
                .Include(v => v.VideoUrl)
                .Include(v => v.Comment)
                .Include(v => v.Attitude)
                .Where(item => string.IsNullOrEmpty(search) ? true : item.Title.Contains(search))
                .OrderByDescending(v => v.CreatedAt)
                .ToPagedList(page, PAGE_SIZE);
            return View(video);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var video = await _context.Video
                .Include(v => v.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (video == null)
                return NotFound();
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var video = await _context.Video.FindAsync(id);
            if (video == null)
                return NotFound();
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
