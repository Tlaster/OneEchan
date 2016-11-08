using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Core.Models.Interfaces
{
    public interface IPagedModel
    {
        int CurrentPage { get; set; }
        int MaxPage { get; set; }
        int ItemCount { get; set; }
        int MaxCount { get; set; }
    }
}
