using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Common.Helpers;
using OneEchan.Server.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneEchan.Server.Controllers
{
    [Route("{language:regex(zh-TW|en-US|ja-JP|ru-RU)}/[controller]/[action]")]
    public class ApiController : Controller
    {
        private const int PAGESIZE = 20;

        private AnimateDatabaseContext _context;
        public ApiController(AnimateDatabaseContext context)
        {
            _context = context;
        }
        public PagedListResult<ListResult> List(string language, int page = 0) 
            => _context.AnimateList.OrderByDescending(item => item.UpdatedAt).Select(item => new ListResult { ID = item.Id, Updated_At = item.UpdatedAt, Name = LanguageHelper.GetLanguegeName(language, item) }).ToPagedList(page, PAGESIZE);

        public DetailResult Detail(string language, int id)
        {
            if (!_context.AnimateList.Any(item => item.Id == id))
                return null;
            return new DetailResult
            {
                ID = id,
                Name = LanguageHelper.GetLanguegeName(language, _context.AnimateList.FirstOrDefault(item => item.Id == id)),
                List = _context.SetDetail.Where(item => item.Id == id).OrderBy(item => item.SetName).Select(item => new DetailList { ClickCount = item.ClickCount, Created_At = item.Created_At, FileThumb = item.FileThumb, Set = item.SetName }).ToList(),
            };
        }

        public SetResult Watch(string language,int id, double set)
        { 
            if (!_context.AnimateList.Any(anime => anime.Id == id))
                return null;
            var item = _context.SetDetail.FirstOrDefault(anime => anime.Id == id && anime.SetName == set);
            if (item == null)
                return null;
            item.ClickCount++;
            _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return new SetResult { HighQuality = item.HighQuality, LowQuality = item.LowQuality, MediumQuality = item.MediumQuality, OriginalQuality = item.OriginalQuality ?? item.FilePath };
        }

        public PagedListResult<ListResult> Search(string language, string keyword, int page = 0) 
            => _context.AnimateList.Where(item => (item.EnUs != null && item.EnUs.Contains(keyword)) || (item.JaJp != null && item.JaJp.Contains(keyword)) || (item.RuRu != null && item.RuRu.Contains(keyword)) || (item.ZhTw != null && item.ZhTw.Contains(keyword))).OrderByDescending(item => item.UpdatedAt).Select(item => new ListResult { ID = item.Id, Updated_At = item.UpdatedAt, Name = LanguageHelper.GetLanguegeName(language, item) }).ToPagedList(page, PAGESIZE);
    }
}
