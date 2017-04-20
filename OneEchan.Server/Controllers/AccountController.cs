using cloudscribe.Core.Identity;
using cloudscribe.Core.Models;
using cloudscribe.Core.Web;
using cloudscribe.Core.Web.ViewModels.SiteUser;
using cloudscribe.Web.Common;
using cloudscribe.Web.Common.Extensions;
using cloudscribe.Web.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OneEchan.Server.Data;
using OneEchan.Server.Models;
using OneEchan.Server.Models.AccountViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private readonly ISiteContext _currentSite;
        private readonly SiteUserManager<SiteUser> _userManager;
        private readonly SiteSignInManager<SiteUser> _signInManager;
        //private readonly IAuthEmailSender emailSender;
        private readonly ISmsSender _smsSender;
        private IStringLocalizer _sr;
        private ITimeZoneIdResolver _timeZoneIdResolver;
        private ITimeZoneHelper _tzHelper;
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 20;

        public AccountController(
            ApplicationDbContext context,
            SiteContext currentSite,
            SiteUserManager<SiteUser> userManager,
            SiteSignInManager<SiteUser> signInManager,
            ISmsSender smsSender,
            IStringLocalizer<CloudscribeCore> localizer,
            ITimeZoneIdResolver timeZoneIdResolver,
            ITimeZoneHelper timeZoneHelper)
        {
            _context = context;
            _currentSite = currentSite;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._smsSender = smsSender;
            _sr = localizer;
            this._timeZoneIdResolver = timeZoneIdResolver;
            _tzHelper = timeZoneHelper;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            return base.View(new Models.AccountViewModel.AccountIndexViewModel
            {
                Name = user.DisplayName,
                AvaterUrl = user.AvatarUrl,
                Gender = user.Gender,
                Description = user.Comment,
                WebSiteUrl = user.WebSiteUrl,
            });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            if (user != null)
            {
                user.DisplayName = model.Name;
                user.Gender = model.Gender;
                user.WebSiteUrl = model.WebSiteUrl;
                user.Comment = model.Description;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    this.AlertSuccess(_sr["Your profile has been changed."]);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    this.AlertDanger(string.Join(@"<br/>", result.Errors.Select(item => item.Description)));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Security()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    this.AlertSuccess(_sr["Your password has been changed."]);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    this.AlertDanger(string.Join(@"<br/>", result.Errors.Select(item => item.Description)));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Video(int page = 0, string search = "")
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var video = _context.Video
                .Include(v => v.VideoUrl)
                .Include(v => v.Comment)
                .Include(v => v.Attitude)
                .Where(item => item.UploaderId == user.Id && string.IsNullOrEmpty(search) ? true : item.Title.Contains(search))
                .OrderByDescending(v => v.CreatedAt)
                .ToPagedList(page, PAGE_SIZE);
            return View(video);
        }

        public async Task<IActionResult> Article(int page = 0, string search = "")
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var arricle = _context.Article
                .Include(a => a.Comment)
                .Include(a => a.Attitude)
                .Where(item => item.UploaderId == user.Id && string.IsNullOrEmpty(search) ? true : item.Title.Contains(search))
                .OrderByDescending(a => a.CreatedAt)
                .ToPagedList(page, PAGE_SIZE);
            return View(arricle);
        }


    }

}
