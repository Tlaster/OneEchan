using System;
using System.Collections.Generic;

namespace OneEchan.Server.Models
{
    public partial class SetDetail
    {
        public int Id { get; set; }
        public double SetName { get; set; }
        public string FilePath { get; set; }
        public int ClickCount { get; set; }
        public string FileThumb { get; set; }
        public string HighQuality { get; set; }
        public string LowQuality { get; set; }
        public string MediumQuality { get; set; }
        public string OriginalQuality { get; set; }
        public DateTime? Created_At { get; set; }

        public virtual AnimateList IdNavigation { get; set; }
    }
}
