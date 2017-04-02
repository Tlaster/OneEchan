using cloudscribe.Core.Identity;
using cloudscribe.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ArticleController(ApplicationDbContext context, SiteUserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public Task<IActionResult> Edit(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Edit(PostEditViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
