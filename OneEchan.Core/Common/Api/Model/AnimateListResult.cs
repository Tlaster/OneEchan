using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OneEchan.Core.Common.Api.Model
{
    
    public class AnimateListResult
    {
        [JsonProperty]
        public IEnumerable<AnimateListModel> List { get; internal set; }
        [JsonProperty]
        public bool HasMore { get; internal set; }
        [JsonProperty]
        public bool Success { get; internal set; }
    }
}
