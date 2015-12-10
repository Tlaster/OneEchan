using System;
using System.Runtime.Serialization;

namespace AnimateRaw.Shared.Model
{
    [DataContract]
    public class AnimateListModel
    {
        [DataMember]
        public double ID { get; internal set; }
        [IgnoreDataMember]
        public TimeSpan LastUpdate { get; internal set; }
        [DataMember]
        public string Name { get; internal set; }
    }

}
