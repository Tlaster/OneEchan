using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneEchan.Server.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //[Required]
        //public Guid SiteId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        
        //public virtual ICollection<CategoryName> CategoryName { get; set; }
        public virtual ICollection<Post> Post { get; set; }
    }
}