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
    public class Detail
    {
        public static async Task<AnimateInfoModel> GetDetail(int id, string prefLang = "JP")
        {
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync($"http://oneechan.moe/api/detail?id={id}&prefLang={prefLang}");
                return JsonConvert.DeserializeObject<AnimateInfoModel>(jsstr);
            }
        }

        public static async Task<string> AddClick(int id, int fileName, string prefLang = "JP")
        {

            using (var client = new HttpClient())
                return ((JObject)JsonConvert.DeserializeObject(await client.GetStringAsync($"http://oneechan.moe/api/detail?id={id}&filename={fileName}&prefLang={prefLang}"))).Value<string>("FilePath");
        }
    }
}
