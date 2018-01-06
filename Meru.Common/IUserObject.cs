using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Common
{
    public interface IUser : IEntity
    {
		string Name { get; }
		string AvatarUrl { get; }

		bool IsBot { get; }
		bool IsSelf { get; }


    }
}
