using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EN
{
	public class EnLanguageDataProvider : ILanguageDataProvider
	{
		#region Private Static Fields
		private static string[] _phoneticHeaps = { "aeiouy", "bp", "ckq", "dt", "lr", "mn", "gj", "fpv", "sxz", "csz" };
		private static Dictionary<char, HashSet<char>> _groups;
		private static Dictionary<char, byte> _keyCodes = new Dictionary<char, byte>
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
		#endregion

		public Dictionary<char, HashSet<char>> PhoneticsGroups
		{
			get
			{
				if (_groups == null)
				{
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
		public Dictionary<char, byte> KeyCodes => _keyCodes;
		public Dictionary<char, string> Translit => null;
	}
}
