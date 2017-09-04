using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ILanguageDataProvider
    {
		Dictionary<char, HashSet<char>> PhoneticsNearest { get; }
		Dictionary<char, HashSet<char>> KeyboardNearest { get; }
		Dictionary<char, byte> KeyCodes { get; }
		Dictionary<byte, char[]> CharsByKeycode { get; }
		Dictionary<string, string[]> TranslitFromEn { get; }
		Dictionary<string, string[]> TranslitToEn { get; }
		char[] Alphabet { get; }
	}
}
