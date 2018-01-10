using Meru.Commands.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands.Attributes
{
    public class MultiCommandAttribute : CommandAttribute
    {
		public MultiCommandAttribute()
		{
			Entity = new MultiCommand(Entity);
		}
	}
}
