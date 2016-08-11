using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Core.Common.Api.Model
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
}
