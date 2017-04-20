using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneEchan.Server.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunPolicyModel
    {
        [JsonProperty("expiration")]
        public string Expiration => DateTime.UtcNow.AddSeconds(30).FormatIso8601Date();
        [JsonProperty("conditions")]
        public JArray Conditions { get; }

        public AliyunPolicyModel(string dir, string callback = "", int size = 5)
        {
            Conditions = new JArray
            {
                new JArray
                {
                    "content-length-range", 0, Math.Pow(1024, 3) * size
                },
                new JArray
                {
                     "starts-with", "$key", dir
                },
                new JObject(new JProperty("callback", callback))
            };
        }
        public override string ToString()
            => JsonConvert.SerializeObject(this).ToBase64();
    }

}
