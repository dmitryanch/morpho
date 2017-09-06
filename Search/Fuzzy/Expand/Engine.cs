using Core.Ext;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fuzzy.Expand
{
	public sealed class Engine : EngineBase
	{
		#region Private Fields
		private Func<char, IEnumerable<char>>[] _getShortAlphabets;
		private Func<char, IEnumerable<char>>[] _getWholeAlphabet;
		private bool _useOptimization;
		#endregion

		#region Public API Override
		public override void InitFuzzy(IStrict strictEngine, ILanguageDataProvider langDataProvider, object settings)
		{
			var config = ((int EditDistance, bool Transliterate, bool ConvertByKeycodes, bool UseOptimization))settings;
			base.InitFuzzyBase(strictEngine, langDataProvider, (EditDistance: config.EditDistance, Transliterate: config.Transliterate, ConvertByKeycodes: config.ConvertByKeycodes));
			_useOptimization = config.UseOptimization;
			_getShortAlphabets = new Func<char, IEnumerable<char>>[]
				{ c => _langDataProvider.PhoneticsNearest[c], c => _langDataProvider.KeyboardNearest[c] };
			_getWholeAlphabet = new Func<char, IEnumerable<char>>[] { c => _langDataProvider.Alphabet };
		}
		#endregion

		#region Override Methods
		internal protected override string[] FindCorrections(string key)
		{
			return Find(key).Where(w => _strict.Contains(w)).ToArray();
		}

		internal protected override (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] FindCorrectionsInfo(string key)
		{
			return Find(key).Select(w => (Correction: w, Info: _strict.Get(w))).Where(c => c.Info != null).ToArray();
		}

		internal protected override async Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> FindCorrectionsInfoAsync(string key, CancellationToken token)
		{
			return (await FindAsync(key, token)).Select(w => (Correction: w, Info: _strict.Get(w))).Where(c => c.Info != null).ToArray();
		}
		#endregion Override Methods

		#region Private Methods
		private string[] Find(string key)
		{
			return Find(key, _useOptimization ? _getShortAlphabets : _getWholeAlphabet);
		}

		private async Task<string[]> FindAsync(string key, CancellationToken token)
		{
			return await Task.Factory.StartNew(() =>
				Find(key, _useOptimization
					? new Func<char, IEnumerable<char>>[] {c => token.IsCancellationRequested ? new HashSet<char>() : _langDataProvider.PhoneticsNearest[c],
						c => token.IsCancellationRequested ? new HashSet<char>() : _langDataProvider.KeyboardNearest[c] }
					: new Func<char, IEnumerable<char>>[] { c => token.IsCancellationRequested ? new char[0] : _langDataProvider.Alphabet }), token);
		}

		private string[] Find(string key, Func<char, IEnumerable<char>>[] getAlphabets)
		{
			var corrections = ExpandQuery(key);
			var extra = new string[corrections.Length];
			Array.Copy(corrections, extra, corrections.Length);
			for (var i = 0; i < _editDistance; i++)
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
					result = result.Add(key.Insert(i, s.ToString()));
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
					result = result.Add($"{begining ?? string.Empty}{key[i + 1]}{key[i]}{transpositionEnding ?? string.Empty}");
				}

				// removals (key.Length worst)
				result = result.Add(key.Remove(i, 1));

				// replacements (key.Length * Alphabet.Length worst)
				var replacementEnding = i + 1 < key.Length ? key.Substring(i + 1) : null;
				foreach (var s in alphabet)
				{
					if (s == key[i])
					{
						continue;
					}
					result = result.Add($"{begining ?? string.Empty}{s}{replacementEnding ?? string.Empty}");
				}
			}
			return result;
		}
		#endregion
	}
}
