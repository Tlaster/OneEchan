using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Interface
{
    public interface IItemsView
    {
        string Title { get; }
        string Caption { get; }
        string Thumb { get; }
        string Action { get; }
        string Controller { get; }
        object Values { get; }
    }
}
