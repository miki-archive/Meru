using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Common
{
    public interface IGuildObject : IEntityObject
    {
        string Name { get; }
    }
}
