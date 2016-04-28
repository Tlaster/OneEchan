using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OneEchan.Shared;
using OneEchan.Shared.Common.Helper;

namespace OneEchan.Common
{
    public class AnimateListIncrementalLoadingClass : IncrementalLoadingBase
    {
        private bool _hasMore = true;
        private int _page = 0;

        protected override bool HasMoreItemsOverride() => _hasMore;

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            var item = await Core.Common.Api.Home.GetList(_page++, LanguageHelper.PrefLang);
            if (!item.Success)
                return null;
            _hasMore = item.HasMore;
            return item.List.ToArray();
        }
    }
}
