using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Common.Extensions
{
    public static class IEnumerableExtension
    {
        private static Random _random { get; } = new Random();
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            var list = enumerable as IList<T> ?? enumerable.ToList();
            lock (_random)
                return list.Count == 0 ? default(T) : list[_random.Next(0, list.Count)];
        }
    }
}
