using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.Aliyun
{
    public class AliyunCallbackModel
    {
        public string FileName { get; set; }
        public string Size { get; set; }
        public string MimeType { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public override string ToString()
        {
            return $"filename=${FileName}&size=${Size}&mimeType=${MimeType}&height=${Height}&width=${Width}";
        }
    }
}
