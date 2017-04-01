using cloudscribe.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.PostViewModel
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public SiteUser Uploader { get; set; }
        public long LikeCount { get; set; }
        public long TotalAttitudeCount { get; set; }
    }
}
