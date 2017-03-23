using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class VideoUrl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string QualityInfo { get; set; }
        public string Url { get; set; }
        public TimeSpan Duration { get; set; }
        public string Thumb { get; set; }

        public virtual Video Video { get; set; }
    }
}
