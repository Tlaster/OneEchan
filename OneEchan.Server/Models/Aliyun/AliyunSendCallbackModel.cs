using Newtonsoft.Json;
using OneEchan.Server.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunSendCallbackModel
    {
        public AliyunSendCallbackModel(string callbackUrl)
        {
            CallbackUrl = callbackUrl;
        }
        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }
        [JsonProperty("callbackBody")]
        public string CallbackBody { get; set; } = "filename=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}&height=${imageInfo.height}&width=${imageInfo.width}&userid=${x:userid}";
        [JsonProperty("callbackBodyType")]
        public string CallbackBodyType { get; set; } = "application/x-www-form-urlencoded";
        [JsonProperty("callbackHost")]
        public string CallbackHost => new Uri(CallbackUrl).Host;
        public override string ToString() 
            => JsonConvert.SerializeObject(this).ToBase64();
        
    }             
}
