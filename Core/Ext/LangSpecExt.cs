using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Ext
{
    public static class LangSpecExt
    {
		public static byte[] GetKeyCodes(this string word, Dictionary<char, byte> keycodes)
		{
			return word.ToLower().Select(ch => keycodes[ch]).ToArray();
		}
	}
}
