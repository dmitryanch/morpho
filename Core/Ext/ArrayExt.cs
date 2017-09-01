using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Ext
{
	public static class ArrayExt
	{
		public static T[] Add<T>(this T[] array, T newItem)
		{
			return (array ?? Enumerable.Empty<T>()).Concat(Enumerable.Repeat(newItem, 1)).ToArray();
		}
	}
}
