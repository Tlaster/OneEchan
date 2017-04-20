using Aliyun.Acs.Sts.Model.V20150401;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunStsSignatureModel : AssumeRoleResponse.Credentials_
    {
        public AliyunStsSignatureModel(AssumeRoleResponse.Credentials_ credentials, AliyunOptions aliyunOptions)
        {
            AccessKeyId = credentials.AccessKeyId;
            AccessKeySecret = credentials.AccessKeySecret;
            Expiration = credentials.Expiration;
            SecurityToken = credentials.SecurityToken;
            Bucket = aliyunOptions.Bucket;
            Endpoint = aliyunOptions.Endpoint;
        }

        public string Endpoint { get; set; }
        public string Bucket { get; set; }
    }
}
