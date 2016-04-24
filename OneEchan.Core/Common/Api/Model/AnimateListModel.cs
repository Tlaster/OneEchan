using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;

namespace OneEchan.Core.Common.Api.Model
{
    public class AnimateListModel
    {
        [JsonProperty]
        public int ID { get; internal set; }
        [JsonProperty]
        public string LastUpdate { get; internal set; }

        [JsonIgnore]
        public TimeSpan LastUpdateTime
        {
            get
            {
                var zone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
                var zoned = LocalDateTime.FromDateTime(DateTime.Parse(LastUpdate)).InZoneStrictly(zone);
                var utc = zoned.WithZone(DateTimeZone.Utc).ToDateTimeUtc();
                return DateTime.UtcNow - utc;
            }
        }
        [JsonProperty]
        public string Name { get; internal set; }
    }
}
