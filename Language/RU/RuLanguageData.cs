using Core.Interfaces;
using Core.Keyboard;
using System.Collections.Generic;
using System.Linq;

namespace RU
{
	public class RuLanguageData : ILanguageData
	{
		#region Private Static Fields
		private static string[] _phoneticHeaps = { "аоуэыиеёя", "сзц", "тсц", "тд", "шжщч", "лйр", "вф", "бп", "гкх", "мн" };
		private static Dictionary<char, HashSet<char>> _phoneticGroups;
		private static Dictionary<char, HashSet<char>> _keyboardGroups;
		private static Dictionary<char, byte> _keycodes = new Dictionary<char, byte>
		{
			{ 'ё' , 192 },
			{ '1' , 49  },
			{ '2' , 50  },
			{ '3' , 51  },
			{ '4' , 52  },
			{ '5' , 53  },
			{ '6' , 54  },
			{ '7' , 55  },
			{ '8' , 56  },
			{ '9' , 57  },
			{ '0' , 48  },
			{ '-' , 189 },
			{ '=' , 187 },
			{ 'й' , 81  },
			{ 'ц' , 87  },
			{ 'у' , 69  },
			{ 'к' , 82  },
			{ 'е' , 84  },
			{ 'н' , 89  },
			{ 'г' , 85  },
			{ 'ш' , 73  },
			{ 'щ' , 79  },
			{ 'з' , 80  },
			{ 'х' , 219 },
			{ 'ъ' , 221 },
			{ 'ф' , 65  },
			{ 'ы' , 83  },
			{ 'в' , 68  },
			{ 'а' , 70  },
			{ 'п' , 71  },
			{ 'р' , 72  },
			{ 'о' , 74  },
			{ 'л' , 75  },
			{ 'д' , 76  },
			{ 'ж' , 186 },
			{ 'э' , 222 },
			{ 'я' , 90  },
			{ 'ч' , 88  },
			{ 'с' , 67  },
			{ 'м' , 86  },
			{ 'и' , 66  },
			{ 'т' , 78  },
			{ 'ь' , 77  },
			{ 'б' , 188 },
			{ 'ю' , 190 },
			{ '.' , 191 },

			{ '!' , 49  },
			{ '"' , 50  },
			{ '№' , 51  },
			{ ';' , 52  },
			{ '%' , 53  },
			{ ':' , 54  },
			{ '?' , 55  },
			{ '*' , 56  },
			{ '(' , 57  },
			{ ')' , 48  },
			{ '_' , 189 },
			{ '+' , 187 },
			{ ',' , 191 },
			{ '\'' , 222  },
			{ (char)8217 , 222  },
		};
		private static Dictionary<byte, char[]> _charsByKeycode;
		private static Dictionary<string, string[]> _translitToEn = new Dictionary<string, string[]>
		{
			{ "ий", new [] { "y", "iy"} },
			{ "а", new [] { "a" } },
			{ "б", new [] { "b", "6" } },
			{ "в", new [] { "v", "b", "8" } },
			{ "г", new [] { "g", "r" } },
			{ "д", new [] { "d", "g" } },
			{ "е", new [] { "e", "3" } },
			{ "ё", new [] { "yo", "jo" } },
			{ "ж", new [] { "zh" } },
			{ "з", new [] { "z", "3" } },
			{ "и", new [] { "i", "u" } },
			{ "й", new [] { "i", "" } },
			{ "к", new [] { "k" } },
			{ "л", new [] { "l" } },
			{ "м", new [] { "m" } },
			{ "н", new [] { "n", "h" } },
			{ "о", new [] { "о", "o", "0" } },
			{ "п", new [] { "p", "n" } },
			{ "р", new [] { "r", "p" } },
			{ "с", new [] { "s", "c" } },
			{ "т", new [] { "t", "m" } },
			{ "у", new [] { "u", "y" } },
			{ "ф", new [] { "f" } },
			{ "х", new [] { "x", "h" } },
			{ "ц", new [] { "c", "ts" } },
			{ "ч", new [] { "ch", "4" } },
			{ "ш", new [] { "sh", "w" } },
			{ "щ", new [] { "shh", "sh'", "sh", "w"} },
			{ "ъ", new [] { "" } },
			{ "ы", new [] { "y" } },
			{ "ь", new [] { "'", "`" } },
			{ "э", new [] { "e", "je" } },
			{ "ю", new [] { "yu", "ju" } },
			{ "я", new [] { "ya", "ja", "q" } },
			{ "-", new [] { "-" } },
			{ "\'", new [] { "\'", ((char)8217).ToString() } },
			{ ((char)8217).ToString(), new [] { "\'", ((char)8217).ToString() } }
		}.OrderByDescending(k => k.Key.Length).ToDictionary(k => k.Key, k => k.Value);
		private static char[] _alphabet = _translitToEn.Keys.Where(k => k.Length == 1).Select(s => s[0]).ToArray();
		private static Dictionary<string, string[]> _translitFromEn;
		#endregion

		public Dictionary<char, HashSet<char>> PhoneticsNearest
		{
			get
			{
				if (_phoneticGroups == null)
				{
					_phoneticGroups = new Dictionary<char, HashSet<char>>();
					foreach (string group in _phoneticHeaps)
						foreach (char symbol in group)
							if (!_phoneticGroups.ContainsKey(symbol))
								_phoneticGroups.Add(symbol, new HashSet<char>(_phoneticHeaps.Where(pg => pg.Contains(symbol))
									.SelectMany(pg => pg).Distinct().Where(ch => ch != symbol)));
				}
				return _phoneticGroups;
			}
		}
		public Dictionary<char, HashSet<char>> KeyboardNearest
		{
			get
			{
				if (_keyboardGroups == null)
				{
					_keyboardGroups = new Dictionary<char, HashSet<char>>();
					foreach (var keycode in _keycodes)
					{
						var nearestKeyCodes = Qwerty.NearestByKeycode[keycode.Value];
						_keyboardGroups.Add(keycode.Key, new HashSet<char>(_keycodes
							.Where(k => nearestKeyCodes.Contains(k.Value)).Select(k => k.Key)));
					}
				}
				return _keyboardGroups;
			}
		}
		public Dictionary<char, byte> KeyCodes => _keycodes;
		public Dictionary<byte, char[]> CharsByKeycode
		{
			get
			{
				if (_charsByKeycode == null)
				{
					_charsByKeycode = KeyCodes.Values.Select(b => b).Distinct()
						.ToDictionary(c => c, c => KeyCodes.Where(k => k.Value == c).Select(k => k.Key).ToArray());
				}
				return _charsByKeycode;
			}
		}
		public Dictionary<string, string[]> TranslitToEn => _translitToEn;
		public Dictionary<string, string[]> TranslitFromEn
		{
			get
			{
				if (_translitFromEn == null && TranslitToEn != null)
				{
					_translitFromEn = TranslitToEn.Values.SelectMany(s => s).Where(s => s.Length > 0).Distinct().OrderByDescending(s => s.Length)
						.ToDictionary(s => s, s => TranslitToEn.Where(t => t.Value.Contains(s)).Select(t => t.Key).ToArray());
				}
				return _translitFromEn;
			}
		}
		public char[] Alphabet => _alphabet;

		public string Title => "RU";
	}
}
