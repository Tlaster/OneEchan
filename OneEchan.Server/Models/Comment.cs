using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class Comment
    {
        public Comment()
        {
            CreatedAt = DateTime.UtcNow;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Column(name: "Created_At")]
        public DateTime CreatedAt { get; set; }
        [Required]
        public Guid UploaderId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int Language { get; set; }
        [Required]
        public string Ip { get; set; }

        public virtual Post Post { get; set; }
    }
}
