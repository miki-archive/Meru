using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Common
{
    public interface IEntityObject
    {
        object Id { get; }
        Type OriginalIdType { get; }

        DateTimeOffset CreatedAt { get; }
    }
}
