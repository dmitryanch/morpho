using Core.Interfaces;
using Core.Keyboard;
using System.Collections.Generic;
using System.Linq;

namespace EN
{
	public class EnLanguageData : ILanguageData
	{
		#region Private Static Fields
		private static string[] _phoneticHeaps = { "aeiouy", "bp", "ckq", "dt", "lr", "mn", "gj", "fpv", "sxz", "csz" };
		private static Dictionary<char, HashSet<char>> _phoneticGroups;
		private static Dictionary<char, HashSet<char>> _keyboardGroups;
		private static Dictionary<char, byte> _keycodes = new Dictionary<char, byte>
		{
			{ '`', 192 },
			{ '1', 49   },
			{ '2', 50   },
			{ '3', 51   },
			{ '4', 52   },
			{ '5', 53   },
			{ '6', 54   },
			{ '7', 55   },
			{ '8', 56   },
			{ '9', 57   },
			{ '0', 48   },
			{ '-', 189  },
			{ '=', 187  },
			{ 'q', 81   },
			{ 'w', 87   },
			{ 'e', 69   },
			{ 'r', 82   },
			{ 't', 84   },
			{ 'y', 89   },
			{ 'u', 85   },
			{ 'i', 73   },
			{ 'o', 79   },
			{ 'p', 80   },
			{ '[', 219  },
			{ ']', 221  },
			{ 'a', 65   },
			{ 's', 83   },
			{ 'd', 68   },
			{ 'f', 70   },
			{ 'g', 71   },
			{ 'h', 72   },
			{ 'j', 74   },
			{ 'k', 75   },
			{ 'l', 76   },
			{ ';', 186  },
			{ '\'', 222 },
			{ 'z', 90   },
			{ 'x', 88   },
			{ 'c', 67   },
			{ 'v', 86   },
			{ 'b', 66   },
			{ 'n', 78   },
			{ 'm', 77   },
			{ ',', 188  },
			{ '.', 190  },
			{ '/', 191  },

			{ '~' , 192 },
			{ '!' , 49  },
			{ '@' , 50  },
			{ '#' , 51  },
			{ '$' , 52  },
			{ '%' , 53  },
			{ '^' , 54  },
			{ '&' , 55  },
			{ '*' , 56  },
			{ '(' , 57  },
			{ ')' , 48  },
			{ '_' , 189 },
			{ '+' , 187 },

			{ '{', 219  },
			{ '}', 221  },
			{ ':', 186  },
			{ '"', 222  },

			{ '<', 188  },
			{ '>', 190  },
			{ '?', 191  },
		};
		private static Dictionary<byte, char[]> _charsByKeycode;
		private static char[] _alphabet = new char[] { };
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
		public char[] Alphabet => _alphabet;
		public Dictionary<string, string[]> TranslitFromEn => null;
		public Dictionary<string, string[]> TranslitToEn => null;

		public string Title => "EN";
	}
}
