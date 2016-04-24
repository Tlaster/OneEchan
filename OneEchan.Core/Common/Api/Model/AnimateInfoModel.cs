using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OneEchan.Core.Common.Api.Model
{
    public class AnimateInfoModel
    {
        [JsonProperty]
        public bool Success { get; internal set; }
        [JsonProperty]
        public string Name { get; internal set; }
        [JsonProperty]
        public List<AnimateSetModel> SetList { get; internal set; }
    }
}
