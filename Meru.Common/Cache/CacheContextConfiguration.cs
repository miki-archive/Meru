using System;

namespace Meru.Common.Cache
{
    public class CacheConfiguration
    {
        public TimeSpan ExpirationSpan { get; set; } = new TimeSpan(0, 10, 0);
        public bool RefreshEntries { get; set; } = true;
        public bool ShowWarnings { get; set; } = true;
    }
}