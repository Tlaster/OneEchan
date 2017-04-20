using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.VideoViewModel
{
    public class UploadCallbackModel
    {
        [Required]
        public string ObjectName { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string Bucket { get; set; }
    }
}
