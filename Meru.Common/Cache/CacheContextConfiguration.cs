using System;

namespace Meru.Common.Cache
{
    public class CacheConfiguration
    {
        public TimeSpan ExpirationSpan = new TimeSpan(0, 10, 0);
        public bool RefreshEntries = true;
        public bool ShowWarnings = true;
    }
}