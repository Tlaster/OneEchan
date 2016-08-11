using System;
using System.Collections.Generic;

namespace OneEchan.Server.Models
{
    public partial class AnimateList
    {
        public AnimateList()
        {
            SetDetail = new HashSet<SetDetail>();
        }

        public int Id { get; set; }
        public string EnUs { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string JaJp { get; set; }
        public string ZhTw { get; set; }
        public string RuRu { get; set; }

        public virtual ICollection<SetDetail> SetDetail { get; set; }
    }
}
