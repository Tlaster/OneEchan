using cloudscribe.Core.Models;
using OneEchan.Server.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class Video : Post
    {
        public Video() : base()
        {
        }
        public string Description { get; set; }
        public virtual ICollection<VideoUrl> VideoUrl { get; set; }
    }
}
