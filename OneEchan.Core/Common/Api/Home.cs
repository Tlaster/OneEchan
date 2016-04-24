using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneEchan.Core.Common.Api.Model;

namespace OneEchan.Core.Common.Api
{
    public class Home
    {
        public static async Task<AnimateListResult> GetList(int page = 0,string prefLang = "JP")
        {
            string _serverLink = $"http://oneechan.moe/api/list?page={page}&prefLang={prefLang}";
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync(_serverLink);
                return JsonConvert.DeserializeObject<AnimateListResult>(jsstr);
            }
        }
    }
}
