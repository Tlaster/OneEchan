using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.PostViewModel
{
    public abstract class PostEditModel
    {
        public int Id { get; set; }
        public bool AllowComment { get; set; }
        [Required]
        [StringLength(140, MinimumLength = 6, ErrorMessage = "This field must between 6 and 140 characters")]
        public string Title { get; set; }

        public int CategoryId { get; set; }
    }
    public class VideoEditModel : PostEditModel
    {
        public string Description { get; set; }
    }
    public class ArticleEditModel : PostEditModel
    {
        public string Content { get; set; }
    }
}
