using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Interfaces
{
    public interface ICardItemModel
    {
        string Text { get; }
        string SubText { get; }
        string Thumb { get; }
        string Href { get; }
    }
}
