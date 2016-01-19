using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AnimateRaw.Shared.Model;
using System.Net.Http;
using Windows.Data.Json;
using AnimateRaw.Extension;

namespace AnimateRaw.Common
{
    public class AnimateListIncrementalLoadingClass : IncrementalLoadingBase
    {
        private bool _hasMore = true;
        private int _page = 0;

        protected override bool HasMoreItemsOverride() => _hasMore;

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync($"http://oneechan.moe/api/list?page={_page++}");
                var obj = JsonObject.Parse(jsstr);
                _hasMore = obj.GetNamedBoolean("HasMore");
                if (obj.GetNamedBoolean("Success"))
                {
                    return (from item in obj.GetNamedArray("List")
                            select new AnimateListModel
                            {
                                ID = item.GetNamedNumber("ID"),
                                Name = item.GetNamedString("Name"),
                                LastUpdateBeijing = DateTime.Parse(item.GetNamedString("LastUpdate")),
                            }).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
