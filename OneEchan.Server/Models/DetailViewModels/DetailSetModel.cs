using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneEchan.Core.Models;
using OneEchan.Server.Models.Interfaces;

namespace OneEchan.Server.Models.DetailViewModels
{
    public class DetailSetModel : DetailList, ICardItemModel
    {
        public int AnimeID { get; internal set; }

        public string Href => $"/Watch?id={AnimeID}&set={Set}";

        public string SubText => $"{ClickCount} Views";

        public string Text => Set.ToString();

        public string Thumb => FileThumb;
    }
}
