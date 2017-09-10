using System.Collections.Generic;

namespace Core.Interfaces
{
	public interface ILanguageData
	{
		Dictionary<char, HashSet<char>> PhoneticsNearest { get; }
		Dictionary<char, HashSet<char>> KeyboardNearest { get; }
		Dictionary<char, byte> KeyCodes { get; }
		Dictionary<byte, char[]> CharsByKeycode { get; }
		Dictionary<string, string[]> TranslitFromEn { get; }
		Dictionary<string, string[]> TranslitToEn { get; }
		char[] Alphabet { get; }
		string Title { get; }
	}
}
