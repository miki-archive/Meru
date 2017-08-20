using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace IA.Events
{
    public static class ConcurrentUtils
    {
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary,
            TKey key, Func<TKey, Task<TValue>> valueFactory)
        {      
            if (dictionary.TryGetValue(key, out TValue resultingValue))
            {
                return resultingValue;
            }
            return dictionary.GetOrAdd(key, await valueFactory(key));
        }
    }
}
