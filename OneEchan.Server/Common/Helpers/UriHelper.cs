using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Common.Helpers
{
    internal static class UriHelper
    {
        public static string CheckForHttps(string uri) => uri != null && uri.StartsWith("http://") ? uri.Replace("http://", "https://") : uri;
    }
}
