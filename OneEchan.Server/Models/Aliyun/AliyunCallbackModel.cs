using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunCallbackModel
    {
        public AliyunCallbackModel(string callbackUrl)
        {
            CallbackUrl = callbackUrl;
        }
        public string CallbackUrl { get; set; }
        public string CallbackBody { get; set; } = "filename=${object}&size=${size}&mimeType=${mimeType}";
        public string CallbackBodyType { get; set; } = "application/x-www-form-urlencoded";
    }
}
