using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEchan.Core.Common.Extensions;
using OneEchan.Core.Models;
using OneEchan.Server.Common.Extensions;
using OneEchan.Server.Common.Helpers;
using OneEchan.Server.Data;
using OneEchan.Server.Models;
using OneEchan.Server.Models.HomeViewModels;

namespace OneEchan.Server.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private const int PAGESIZE = 18;
        private AnimateDatabaseContext Context { get; }
        public HomeController(AnimateDatabaseContext context)
        {
            Context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CanBack = false;
            var language = Request.Headers["Accept-Language"];
            return View(new HomeIndex
            {
                Result = Context.AnimateList.OrderByDescending(item => item.Updated_At)
                                               .Select(item => new HomeListModel
                                               {
                                                   ID = item.Id,
                                                   Updated_At = item.Updated_At.ToUtc(),
                                                   Name = LanguageHelper.GetLanguegeName(language, item),
                                                   Thumb = (from detail in Context.SetDetail
                                                            where !string.IsNullOrEmpty(detail.FileThumb) && detail.Id == item.Id
                                                            select detail.FileThumb).Random(),
                                               }).ToPagedList(pageSize: PAGESIZE),
            });
        }

        //private ListResult Selector(AnimateList item, string language)
        //{
        //    return new ListResult
        //    {
        //        ID = item.Id,
        //        Updated_At = item.Updated_At.ToUtc(),
        //        Name = LanguageHelper.GetLanguegeName(language, item),
        //        Thumb = (from detail in Context.SetDetail
        //                 where !string.IsNullOrEmpty(detail.FileThumb) && detail.Id == item.Id
        //                 select detail.FileThumb).Random(),
        //    };
        //}

        public IActionResult More(int page = 0)
        {
            var language = Request.Headers["Accept-Language"];
            return View(new More
            {
                Result = Context.AnimateList.OrderByDescending(item => item.Updated_At)
                                               .Select(item => new HomeListModel
                                               {
                                                   ID = item.Id,
                                                   Updated_At = item.Updated_At.ToUtc(),
                                                   Name = LanguageHelper.GetLanguegeName(language, item),
                                                   Thumb = (from detail in Context.SetDetail
                                                            where !string.IsNullOrEmpty(detail.FileThumb) && detail.Id == item.Id
                                                            select detail.FileThumb).Random(),
                                               }).ToPagedList(page, PAGESIZE),
            });
        }

        public IActionResult Search(string keyword, int page = 0)
        {
            ViewBag.Title = $"OneEchan - {keyword}";
            ViewBag.SearchText = keyword;
            var language = Request.Headers["Accept-Language"];
            return View(new Search
            {
                Keyword = keyword,
                Result = Context.AnimateList.Where(item => (item.EnUs != null && item.EnUs.Contains(keyword)) || (item.JaJp != null && item.JaJp.Contains(keyword)) || (item.RuRu != null && item.RuRu.Contains(keyword)) || (item.ZhTw != null && item.ZhTw.Contains(keyword)))
                                            .OrderByDescending(item => item.Updated_At)
                                            .Select(item => new HomeListModel
                                            {
                                                ID = item.Id,
                                                Updated_At = item.Updated_At.ToUtc(),
                                                Name = LanguageHelper.GetLanguegeName(language, item),
                                                Thumb = (from detail in Context.SetDetail
                                                         where !string.IsNullOrEmpty(detail.FileThumb) && detail.Id == item.Id
                                                         select detail.FileThumb).Random(),
                                            }).ToPagedList(page, PAGESIZE),
            });
        }


        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        public IActionResult Error()
        {
            return View();
        }
    }
}
