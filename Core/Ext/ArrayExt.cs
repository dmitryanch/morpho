using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Ext
{
	public static class ArrayExt
	{
		public static ulong ComputeHash<T>(this T[] array)
		{
			if (array == null) return 0;
			unchecked
			{
				ulong hash = 17;
				for (var i = 0; i < array.Length; i++)
					hash = 31 * hash + (ulong)array[i].GetHashCode();
				return hash;
			}
		}

		public static T[] Add<T>(this T[] array, T newItem)
		{
			return (array ?? Enumerable.Empty<T>()).Concat(Enumerable.Repeat(newItem, 1)).ToArray();
		}

		public static bool Equals<T>(T[] arrayA, T[] arrayB)
		{
			if(arrayA.Length != arrayB.Length)
			{
				return false;
			}
			for(var i = 0; i < arrayA.Length; i++)
			{
				if(arrayA[i].GetHashCode() != arrayB[i].GetHashCode())
				{
					return false;
				}
			}
			return true;
		}
	}
}
