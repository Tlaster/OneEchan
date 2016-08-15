using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Core.Models
{
    public class PagedListResult<T>
    {
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
        public int ItemCount { get; set; }
        public int MaxCount { get; set; }
        public List<T> List { get; set; }
    }
    public class ListResult
    {
        public string Name { get; set; }
        public DateTime? Updated_At { get; set; }
        public int ID { get; set; }
    }
    public static class IEnumerableExtension
    {
        public static PagedListResult<T> ToPagedList<T>(this IEnumerable<T> list, int page = 0, int pageSize = 20)
        {
            var result = list.Skip(page * pageSize).Take(pageSize).ToList();
            return new PagedListResult<T>
            {
                List = result,
                CurrentPage = page,
                ItemCount = result.Count,
                MaxCount = list.Count(),
                MaxPage = (list.Count() + pageSize - 1) / pageSize - 1,
            };
        }
    }

}
