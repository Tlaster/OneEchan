using System;
using System.Runtime.Serialization;

namespace OneEchan.Shared.Model
{
    [DataContract]
    public class AnimateListModel
    {
        [DataMember]
        public double ID { get; internal set; }
        [IgnoreDataMember]
        public DateTime LastUpdateBeijing { get; internal set; }
        [IgnoreDataMember]
#if WINDOWS_UWP
        public TimeSpan LastUpdate => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")) - LastUpdateBeijing;
#else
        public TimeSpan LastUpdate => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT-8")) - LastUpdateBeijing;
#endif
        [DataMember]
        public string Name { get; internal set; }
    }

}
