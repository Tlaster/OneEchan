using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Common.Extensions;
using OneEchan.Server.Common.Helpers;
using OneEchan.Server.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneEchan.Server.Controllers
{
    public class WatchController : Controller
    {
        private AnimateDatabaseContext _context;
        public WatchController(AnimateDatabaseContext context)
        {
            _context = context;
        }
    
        public IActionResult Index(int id, double set)
        {
            if (!_context.AnimateList.Any(anime => anime.Id == id))
                return null;
            var item = _context.SetDetail.FirstOrDefault(anime => anime.Id == id && anime.SetName == set);
            if (item == null)
                return null;
            item.ClickCount++;
            _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            var title = LanguageHelper.GetLanguegeName(Request.Headers["Accept-Language"], _context.AnimateList.FirstOrDefault(amine => amine.Id == id));
            ViewBag.Title = $"{title} - {set}";
            ViewBag.IsMobile = Request.IsMobileBrowser();
            var model = new SetResult { HighQuality = item.HighQuality, LowQuality = item.LowQuality, MediumQuality = item.MediumQuality, OriginalQuality = item.OriginalQuality ?? item.FilePath };
            return View(model);
        }
    }
}
