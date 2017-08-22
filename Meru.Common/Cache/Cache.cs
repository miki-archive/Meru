using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common.Cache
{
    public class Cache<K, V> : ConcurrentDictionary<K, CacheContext<V>>
    {
        private readonly CacheConfiguration _defaultCacheContext = new CacheConfiguration();

        public Cache(CacheConfiguration defaultContext) : base()
        {
            _defaultCacheContext = defaultContext;
        }

        public virtual async Task<V> GetOrInsertAsync(K key, Func<K, Task<V>> cacheContextBuilder)
        {
            CacheContext<V> value = GetOrAdd(key, new CacheContext<V>(await cacheContextBuilder(key), _defaultCacheContext));

            if (!value.Valid && _defaultCacheContext.RefreshEntries)
            {
                V newData = await cacheContextBuilder(key);

                if (TryAdd(key, new CacheContext<V>(newData, _defaultCacheContext)))
                {
                    return newData;
                }
            }
            return value.Data;
        }

        /// TODO: find a way to make this async :thinking:
        public virtual async Task<CacheContext<V>> AddOrUpdateAsync(K key, V value, Func<K, CacheContext<V>, CacheContext<V>> updateContextBuilder)
        {
            return AddOrUpdate(key, new CacheContext<V>(value, _defaultCacheContext), updateContextBuilder);
        }
    }
}
