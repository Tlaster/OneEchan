using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunSignatureModel
    {
        public string AccessId { get; set; }
        public string Host { get; set; }
        public string Policy { get; set; }
        public string Signature { get; set; }
        public string Expire { get; set; }
        public string Callback { get; set; }
        public string Dir { get; set; }
        public string UserId { get; set; }
    }
}
