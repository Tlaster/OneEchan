using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Backend.QCloud.Common
{
    public static class Extension
    {
        public static long ToUnixTime(this DateTime nowTime)
            => new DateTimeOffset(nowTime).ToUnixTimeMilliseconds();

    }
}
