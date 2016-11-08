using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneEchan.Core.Models;
using OneEchan.Server.Models.Interfaces;
using OneEchan.Core.Common.Extensions;

namespace OneEchan.Server.Models.HomeViewModels
{
    public class HomeListModel : ListResult, ICardItemModel
    {
        public string Href => $"/Detail?id={ID}";

        public string SubText => Updated_At?.DiffForHumans();

        public string Text => Name;
    }
}
