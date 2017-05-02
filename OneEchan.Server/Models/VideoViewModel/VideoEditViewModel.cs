using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.VideoViewModel
{
    public class VideoEditViewModel
    {
        public Video Video { get; set; }
        public Category[] Categories { get; set; }
    }
}
