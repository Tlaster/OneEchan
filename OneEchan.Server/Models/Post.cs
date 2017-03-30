using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public abstract class Post
    {
        public enum State
        {
            Published,
            Publishing,
            Blocked,
            UploaderOnly,
            Editing,
        }
        public Post()
        {
            CreatedAt = DateTime.UtcNow;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Column(name: "Created_At")]
        public DateTime CreatedAt { get; set; }
        [Required]
        public Guid UploaderId { get; set; }
        [Required]
        public Guid SiteId { get; set; }
        public State PostState{ get; set; }
        [Required]
        public int Language { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public long ViewCount { get; set; }
        public bool AllowComment { get; set; }
        [Required]
        public string Ip { get; set; }
        public string TagString { get; set; }
        [NotMapped]
        public string[] Tags
        {
            get => TagString.Split(',').ToArray();
            set => TagString = string.Join(",", value);
        }

        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Attitude> Attitude { get; set; }
    }
}
