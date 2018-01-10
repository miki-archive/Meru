using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Common.Utils
{
    public static class ArrayExtensions
    {
		public static T TryGet<T>(this T[] arr, int index)
		{
			if(index < arr.Length)
			{
				return arr[index];
			}
			return default(T);
		}
    }
}
