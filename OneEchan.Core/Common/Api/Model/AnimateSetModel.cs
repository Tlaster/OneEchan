using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OneEchan.Core.Common.Api.Model
{
    public class AnimateSetModel
    {
        [JsonProperty]
        public int ClickCount { get; internal set; }
        [JsonProperty]
        public string FileName { get; internal set; }
        [JsonProperty]
        public string FilePath { get; internal set; }
        [JsonProperty]
        public string FileThumb { get; internal set; }
    }
}
