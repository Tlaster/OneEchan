using OneEchan.Server.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public abstract class Post : IItemsView
    {
        public enum State
        {
            Published,
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
        public int Rating { get; set; }
        public int Language { get; set; }
        public int CategoryId { get; set; }
        public long ViewCount { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }

        public string Title => Name;

        public abstract string Caption { get; }
        public abstract string Action { get; }
        public abstract string Controller { get; }
        public abstract object Values { get; }
        public abstract string Thumb { get; }
    }
}
