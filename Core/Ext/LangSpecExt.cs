using System.Collections.Generic;
using System.Linq;

namespace Core.Ext
{
	public static class LangSpecExt
	{
		public static byte[] GetKeyCodes(this string word, Dictionary<char, byte> defaultKeycodes, Dictionary<char, byte>[] extraKeycodes = null)
		{
			return word.ToLower()
				.Select(ch => defaultKeycodes.TryGetValue(ch, out byte val) 
					? val 
					: extraKeycodes?.FirstOrDefault(kc => kc.ContainsKey(ch))?[ch] ?? (byte)0).ToArray();
		}
	}
}
