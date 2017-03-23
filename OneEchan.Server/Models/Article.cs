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

        public override string Caption => string.Empty;

        public override string Action => nameof(ArticleController.Details);

        public override string Controller => nameof(ArticleController);

        public override object Values => new { id = Id };

        public override string Thumb => string.Empty;
    }
}
