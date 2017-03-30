using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class Attitude
    {
        public enum Type
        {
            Like,
            DisLike
        }
        [Required]
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Type AttitudeType { get; set; }

        public Post Post { get; set; }
    }
}
