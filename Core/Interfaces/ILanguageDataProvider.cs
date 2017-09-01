using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ILanguageDataProvider
    {
		Dictionary<char, HashSet<char>> PhoneticsGroups { get; }
		Dictionary<char, byte> KeyCodes { get; }
		Dictionary<char, string> Translit { get; }
	}
}
