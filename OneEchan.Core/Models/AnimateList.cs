using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OneEchan.Core.Models
{
    public partial class AnimateList
    {
        public AnimateList()
        {
            SetDetail = new HashSet<SetDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string EnUs { get; set; }
        public DateTime? Updated_At { get; set; }
        public string JaJp { get; set; }
        public string ZhTw { get; set; }
        public string RuRu { get; set; }

        public virtual ICollection<SetDetail> SetDetail { get; set; }
    }
}
