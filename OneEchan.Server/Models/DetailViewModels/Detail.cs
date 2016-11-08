using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneEchan.Core.Models;

namespace OneEchan.Server.Models.DetailViewModels
{
    public class Detail
    {
        public string Name { get; internal set; }
        public int AnimeID { get; internal set; }
        public List<DetailSetModel> List { get; internal set; }
    }
}
