using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Common.Helpers;
using OneEchan.Server.Models;
using OneEchan.Server.Common.Extensions;
using OneEchan.Core.Models;
using OneEchan.Core.Common.Extensions;
using OneEchan.Server.Data;
using OneEchan.Server.Models.DetailViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneEchan.Server.Controllers
{
    [RequireHttps]
    public class DetailController : Controller
    {
        private AnimateDatabaseContext _context;
        public DetailController(AnimateDatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            if (!_context.AnimateList.Any(item => item.Id == id))
                return null;
            var title = LanguageHelper.GetLanguegeName(Request.Headers["Accept-Language"], _context.AnimateList.FirstOrDefault(item => item.Id == id));
            var model = new Detail
            {
                AnimeID = id,
                Name = title,
                List = _context.SetDetail.Where(item => item.Id == id).OrderBy(item => item.SetName).Select(item => new DetailSetModel { AnimeID = id, ClickCount = item.ClickCount, Created_At = item.Created_At.ToUtc(), FileThumb = item.FileThumb, Set = item.SetName }).ToList(),
            };
            ViewBag.Title = title;
            ViewBag.IsMobile = Request.IsMobileBrowser();
            return View(model);
        }
    }
}
