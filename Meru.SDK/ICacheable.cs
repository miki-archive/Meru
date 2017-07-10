using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public interface ICacheable<T>
    {
        List<T> Data { get; }

        DateTime LastUpdatedAt { get; set; }

        TimeSpan UpdateSpan { get; set; }

        Task UpdateCache();
    }
}
