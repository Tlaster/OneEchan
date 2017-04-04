using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models.PostViewModel
{
    public interface IPostEditViewModel
    {
        int Id { get; set; }
        bool AllowComment { get; set; }
        [Required]
        [StringLength(140, MinimumLength = 6, ErrorMessage = "This field must between 6 and 140 characters")]
        string Name { get; set; }
    }
    public class VideoEditViewModel : IPostEditViewModel
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public bool AllowComment { get; set; }
        public string Name { get; set; }
    }
    public class ArticleEditViewModel : IPostEditViewModel
    {
        public string Content { get; set; }
        public int Id { get; set; }
        public bool AllowComment { get; set; }
        public string Name { get; set; }
    }
}
