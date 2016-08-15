using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneEchan.Core.Models;

namespace OneEchan.Core.Api
{
    public class Home
    {
        public static async Task<PagedListResult<ListResult>> GetList(int page = 0, string prefLang = "ja-JP")
        {
            using (var client = new HttpClient())
                return JsonConvert.DeserializeObject<PagedListResult<ListResult>>(await client.GetStringAsync($"https://oneechan.moe/{prefLang}/api/list?page={page}"));
        }
    }
}
