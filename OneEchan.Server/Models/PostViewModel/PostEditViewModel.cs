using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.PostViewModel
{
    public abstract class PostEditViewModel
    {
        public int Id { get; set; }
        public bool AllowComment { get; set; }
        [Required]
        [StringLength(140, MinimumLength = 6, ErrorMessage = "This field must between 6 and 140 characters")]
        public string Title { get; set; }
    }
    public class VideoEditViewModel : PostEditViewModel
    {
        public string Description { get; set; }
    }
    public class ArticleEditViewModel : PostEditViewModel
    {
        public string Content { get; set; }
    }
}
