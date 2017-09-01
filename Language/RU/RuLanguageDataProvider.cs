using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RU
{
	public class RuLanguageDataProvider : ILanguageDataProvider
	{
		#region Private Static Fields
		private static string[] _phoneticHeaps = { "аоуэыиеёя", "сзц", "тсц", "тд", "шжщч", "лйр", "вф", "бп", "гкх", "мн" };
		private static Dictionary<char, HashSet<char>> _groups;
		private static Dictionary<char, byte> _keycodes = new Dictionary<char, byte>
		{
			{ 'ё' , 192  },
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
		};
		private static Dictionary<char, string> _translit = new Dictionary<char, string>
		{
			{ 'а', "a" },
			{ 'б', "b" },
			{ 'в', "v" },
			{ 'г', "g" },
			{ 'д', "d" },
			{ 'е', "e" },
			{ 'ё', "yo" },
			{ 'ж', "zh" },
			{ 'з', "z" },
			{ 'и', "i" },
			{ 'й', "i" },
			{ 'к', "k" },
			{ 'л', "l" },
			{ 'м', "m" },
			{ 'н', "n" },
			{ 'о', "o" },
			{ 'п', "p" },
			{ 'р', "r" },
			{ 'с', "s" },
			{ 'т', "t" },
			{ 'у', "u" },
			{ 'ф', "f" },
			{ 'х', "x" },
			{ 'ц', "c" },
			{ 'ч', "ch" },
			{ 'ш', "sh" },
			{ 'щ', "shh" },
			{ 'ъ', "" },
			{ 'ы', "y" },
			{ 'ь', "'" },
			{ 'э', "e" },
			{ 'ю', "yu" },
			{ 'я', "ya" },
		};
		#endregion

		public Dictionary<char, HashSet<char>> PhoneticsGroups
		{
			get
			{
				if (_groups == null) {
					_groups = new Dictionary<char, HashSet<char>>();
					foreach (string group in _phoneticHeaps)
						foreach (char symbol in group)
							if (!_groups.ContainsKey(symbol))
								_groups.Add(symbol, new HashSet<char>(_phoneticHeaps.Where(pg => pg.Contains(symbol))
									.SelectMany(pg => pg).Distinct().Where(ch => ch != symbol)));
				}
				return _groups;
			}
		}
		public Dictionary<char, byte> KeyCodes => _keycodes;
		public Dictionary<char, string> Translit => _translit;
	}
}
