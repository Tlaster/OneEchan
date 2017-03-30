using OneEchan.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class Article : Post
    {
        public Article() : base()
        {

        }
        public string Content { get; set; }
    }
}
