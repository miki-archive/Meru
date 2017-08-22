using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Common.Cache
{
    public class CacheContext<V>
    {
        public V Data { get; set; }

        public TimeSpan ExpirationSpan { get; set; }

        public bool Valid => DateTime.Now > TimeInstantiated + ExpirationSpan;

        public readonly DateTime TimeInstantiated = DateTime.Now;

        public CacheContext(V data)
        {
            Data = data;
        }

        public CacheContext(V data, CacheConfiguration config)
        {
            Data = data;
            ExpirationSpan = config.ExpirationSpan;
        }
    }
}
