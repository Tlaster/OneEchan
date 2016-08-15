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
    public class Detail
    {
        public static async Task<DetailResult> GetDetail(int id, string prefLang = "ja-JP")
        {
            using (var client = new HttpClient())
                return JsonConvert.DeserializeObject<DetailResult>(await client.GetStringAsync($"https://oneechan.moe/{prefLang}/api/detail?id={id}"));
        }

        public static async Task<SetResult> GetVideo(int id, double set, string prefLang = "ja-JP")
        {

            using (var client = new HttpClient())
                return JsonConvert.DeserializeObject<SetResult>(await client.GetStringAsync($"https://oneechan.moe/{prefLang}/api/watch?id={id}&set={set}"));
        }
    }
}
