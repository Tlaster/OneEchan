using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneEchan.Common
{
    public class GeneratorIncrementalLoadingClass<T> : IncrementalLoadingBase
    {
        public GeneratorIncrementalLoadingClass(uint maxCount, Func<int, T> generator)
        {
            _generator = generator;
            _maxCount = maxCount;
        }

        protected async override Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            uint toGenerate = Math.Min(count, _maxCount - _count);
            var values = from j in Enumerable.Range((int)_count, (int)toGenerate)
                         select (object)_generator(j);
            _count += toGenerate;
            return await Task.FromResult<IList<object>>(values.ToArray());
        }

        protected override bool HasMoreItemsOverride() => _count < _maxCount;

        #region State

        Func<int, T> _generator;
        uint _count = 0;
        uint _maxCount;

        #endregion 
    }
}
