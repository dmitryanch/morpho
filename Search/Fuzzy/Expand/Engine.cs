using Core.Ext;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fuzzy.Expand
{
	public class Engine : IFuzzy
	{
		#region Private Fields
		private IStrict _strict;
		private ILanguageDataProvider _langDataProvider;
		private (int EditDistance, bool Translite, bool ConvertByKeycodes) _settings;
		private Func<char, IEnumerable<char>>[] _getShortAlphabets;
		private Func<char, IEnumerable<char>> _getWholeAlphabet;
		#endregion

		#region Public API
		public void InitFuzzy(IStrict strictEngine, ILanguageDataProvider langDataProvider, (int EditDistance, bool Translite, bool ConvertByKeycodes) settings)
		{
			_strict = strictEngine;
			_langDataProvider = langDataProvider;
			_settings = settings;
			_getShortAlphabets = new Func<char, IEnumerable<char>>[]
				{ c => _langDataProvider.PhoneticsNearest[c], c => _langDataProvider.KeyboardNearest[c] };
			_getWholeAlphabet = c => _langDataProvider.Alphabet;
		}

		public string[] GetCorrections(string key)
		{
			return FindCorrections(key);
		}

		public (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get(string key)
		{
			return FindCorrectionsInfo(key);
		}

		public (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get(string key, bool exactEnough)
		{
			var corrections = GetExact(key);
			if (corrections != null && exactEnough)
			{
				return corrections;
			}
			return FindCorrectionsInfo(key);
		}

		public async Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync(string key, CancellationToken token)
		{
			return await FindCorrectionsInfoAsync(key, token);
		}

		public async Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync(string key, bool exactEnough, CancellationToken token)
		{
			var corrections = GetExact(key);
			if (corrections != null && exactEnough)
			{
				return corrections;
			}
			return await FindCorrectionsInfoAsync(key, token);
		}
		#endregion

		#region Private Methods
		private string[] FindCorrections(string key)
		{
			return Find(key).Where(w => _strict.Contains(w)).ToArray();
		}

		private (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] GetExact(string key)
		{
			var exact = _strict.Get(key);
			return exact != null ? new[] { (Correction: key, Info: exact) } : null;
		}

		private (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] FindCorrectionsInfo(string key)
		{
			return Find(key).Select(w => (Correction: w, Info: _strict.Get(w))).Where(c => c.Info != null).ToArray();
		}

		private string[] Find(string key)
		{
			return Find(key, _getShortAlphabets);
		}

		private async Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> FindCorrectionsInfoAsync(string key, CancellationToken token)
		{
			return (await FindAsync(key, token)).Select(w => (Correction: w, Info: _strict.Get(w))).Where(c => c.Info != null).ToArray();
		}

		private async Task<string[]> FindAsync(string key, CancellationToken token)
		{
			return await Task.Factory.StartNew(() =>
				Find(key, c => token.IsCancellationRequested ? new HashSet<char>() : _langDataProvider.PhoneticsNearest[c],
					c => token.IsCancellationRequested ? new HashSet<char>() : _langDataProvider.KeyboardNearest[c]), token);
		}

		private string[] Find(string key, params Func<char, IEnumerable<char>>[] getAlphabets)
		{
			var corrections = new[] { key };
			if (_settings.Translite)
			{
				corrections = corrections.Concat(TransliterateFromEn(key)).Distinct().ToArray();
			}
			if (_settings.ConvertByKeycodes)
			{
				corrections = corrections.Concat(ConvertByKeycodes(key)).Distinct().ToArray();
			}
			var extra = new[] { key };
			for (var i = 0; i < _settings.EditDistance; i++)
			{
				extra = extra.SelectMany(word => FindByQueryExpand(word, getAlphabets)).Distinct().ToArray();
				corrections = corrections.Concat(extra).Distinct().ToArray();
			}
			return corrections;
		}

		/// <summary>
		/// Find all modifications of input string with edit distance equals to 1 
		/// (in worst case 2 * key.Length + (2 * key.Length + 1) * Alphabet.Length) corrections number)
		/// </summary>
		/// <param name="key">The word string</param>
		/// <returns></returns>
		private string[] FindByQueryExpand(string key, Func<char, IEnumerable<char>>[] getAlphabets)
		{
			var result = new string[0];
			for (var i = 0; i < key.Length + 1; i++)
			{
				var alphabet = getAlphabets.SelectMany(getAlphabet => getAlphabet(key[i])).Distinct().ToArray();
				if (!alphabet.Any())
				{
					// Cancellation Requested!
					return result;
				}
				// insertions ((key.Length + 1) * Alphabet.Length worst)
				foreach (var s in alphabet)
				{
					result.Add(key.Insert(i, s.ToString()));
				}
				if (i == key.Length)
				{
					break;
				}

				var begining = i > 0 ? key.Substring(0, i) : null;
				// transpositions (key.Length worst)
				if (key[i] != key[i + 1])
				{
					var transpositionEnding = i + 2 < key.Length ? key.Substring(i + 2) : null;
					result.Add($"{begining ?? string.Empty}{key[i + 1]}{key[i]}{transpositionEnding ?? string.Empty}");
				}

				// removals (key.Length worst)
				result.Add(key.Remove(i, 1));

				// replacements (key.Length * Alphabet.Length worst)
				var replacementEnding = i + 1 < key.Length ? key.Substring(i + 1) : null;
				foreach (var s in alphabet)
				{
					if (s == key[i])
					{
						continue;
					}
					result.Add($"{begining ?? string.Empty}{s}{replacementEnding ?? string.Empty}");
				}
			}
			return result;
		}

		private string[] TransliterateFromEn(string key)
		{
			if (_langDataProvider.TranslitFromEn == null) return null;
			var corrections = new[] { key };
			foreach (var item in _langDataProvider.TranslitFromEn)
			{
				var tempCorrections = new string[0];
				foreach (var word in corrections)
				{
					if (word.Contains(item.Key))
					{
						foreach (var val in item.Value)
						{
							tempCorrections = tempCorrections.Add(word.Replace(item.Key, val));
						}
					}
					else
					{
						tempCorrections = tempCorrections.Add(word);
					}
				}
				corrections = tempCorrections;
			}
			return corrections;
		}

		private string[] ConvertByKeycodes(string key)
		{
			var wordKeycodes = key.GetKeyCodes(_langDataProvider.KeyCodes);
			var words = new string[0];
			for (var i = 0; i < wordKeycodes.Length; i++)
			{
				var tempWords = new string[0];
				var chars = _langDataProvider.CharsByKeycode[wordKeycodes[i]];
				words = chars.SelectMany(c => words.Any() ? words.Select(w => w.Insert(w.Length, c.ToString())) : new[] { c.ToString() }).ToArray();
			}
			return words;
		}
		#endregion
	}
}
