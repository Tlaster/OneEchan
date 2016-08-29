using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Backend.Models
{

    public class CheckModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ItemID { get; set; }
        [Required]
        public string SetName { get; set; }
        [Required]
        public string ZhTW { get; set; }

        public override string ToString()
            => $"{Name} {SetName}";
    }
}
