using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OneEchan.Core.Api;
using OneEchan.Shared;
using OneEchan.Shared.Common.Helper;

namespace OneEchan.Common
{
    public class AnimateListIncrementalLoadingClass : IncrementalLoadingBase
    {
        private int _maxPage = 1;
        private int _page = 0;

        protected override bool HasMoreItemsOverride() => _page < _maxPage;

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            var item = await Home.GetList(_page++, LanguageHelper.PrefLang);
            _maxPage = item.MaxPage;
            return item.List.ToArray();
        }
    }
}
