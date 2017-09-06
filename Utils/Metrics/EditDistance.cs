using Core.Interfaces;
using Core.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Metrics
{
	public class EditDistance
    {
		private Dictionary<char, HashSet<char>>[] _phoneticGroups;

		public EditDistance(ILanguageDataProvider[] languages)
		{
			_phoneticGroups = languages.Select(l => l.PhoneticsNearest).ToArray();
		}

		public int ByDamerauLevenshtein((string Text, byte[] Codes) source, (string Text, byte[] Codes) target, bool fullWord, bool translation)
		{
			if (String.IsNullOrEmpty(source.Text))
			{
				if (String.IsNullOrEmpty(target.Text))
					return 0;
				return target.Text.Length * 2;
			}
			if (String.IsNullOrEmpty(target.Text))
				return source.Text.Length * 2;
			int n = source.Text.Length;
			int m = target.Text.Length;
			int[,] distance = new int[3, m + 1];
			for (var j = 1; j <= m; j++)
				distance[0, j] = j * 2;
			var currentRow = 0;
			for (var i = 1; i <= n; ++i)
			{
				currentRow = i % 3;
				var previousRow = (i - 1) % 3;
				distance[currentRow, 0] = i * 2;
				for (var j = 1; j <= m; j++)
				{
					distance[currentRow, j] = Math.Min(Math.Min(
								distance[previousRow, j] + ((!fullWord && i == n) ? 2 - 1 : 2),
								distance[currentRow, j - 1] + ((!fullWord && i == n) ? 2 - 1 : 2)),
								distance[previousRow, j - 1] + CostDistanceSymbol(source, i - 1, target, j - 1, translation));

					if (i > 1 && j > 1 && source.Text[i - 1] == target.Text[j - 2]
									   && source.Text[i - 2] == target.Text[j - 1])
					{
						distance[currentRow, j] = Math.Min(distance[currentRow, j], distance[(i - 2) % 3, j - 2] + 2);
					}
				}
			}
			return distance[currentRow, m];
		}

		private int CostDistanceSymbol((string Text, byte[] Codes) source, int sourcePosition, (string Text, byte[] Codes) search, int searchPosition, bool translation)
		{
			int[] o = { 3, 4, 5 };
			
			if (source.Text[sourcePosition] == search.Text[searchPosition])
				return 0;
			if (translation)
				return 2;
			if (source.Codes[sourcePosition] != 0 && source.Codes[sourcePosition] == search.Codes[searchPosition])
				return 0;
			int resultWeight = 0;
			if (!Qwerty.DistanceCodeKey.TryGetValue(source.Codes[sourcePosition], out HashSet<byte> nearKeys))
				resultWeight = 2;
			else
				resultWeight = nearKeys.Contains(search.Codes[searchPosition]) ? 1 : 2;
			foreach (var pg in _phoneticGroups)
			{
				if (pg.TryGetValue(search.Text[searchPosition], out HashSet<char> phoneticGroups))
					resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
			}
			return resultWeight;
		}
	}
}
