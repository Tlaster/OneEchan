using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunAccount
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
    }
    public class AliyunOptions
    {
        public AliyunAccount UploadAccount { get; set; }
        public AliyunAccount MtsAccount { get; set; }
        public string RoleArn { get; set; }
        public string RegionId { get; set; }
        public string Bucket { get; set; }
        public string Endpoint { get; set; }
        public string MediaWorkflowId { get; set; }
    }
}
