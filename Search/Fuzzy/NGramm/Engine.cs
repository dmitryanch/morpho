using Core.Classes;
using Core.Ext;
using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils.Metrics;

namespace Search.Fuzzy.NGramm
{
	public sealed class Engine : EngineBase
	{
		#region Private Fields
		private int _N;
		private Dictionary<string, Dictionary<int, string[]>>[] _ngramms;
		private EditDistance _wordDistanceCalculator;
		#endregion

		#region Constructor
		public Engine(IStrict strictEngine, ILanguageDataProvider langDataProvider) : base(strictEngine, langDataProvider)
		{ }
		#endregion

		#region Public API Override
		public override async Task Init(object settings)
		{
			var config = ((int EditDistance, bool Transliterate, bool ConvertByKeycodes, int N))settings;
			await base.InitFuzzyBase((EditDistance: config.EditDistance, Transliterate: config.Transliterate, ConvertByKeycodes: config.ConvertByKeycodes));
			_N = config.N;
			await BuildNGrams(_strict.GetKeys());
			_wordDistanceCalculator = new EditDistance(_otherlanguages.Add(_defaultLanguage));
		}
		#endregion

		#region Override Methods
		internal protected override string[] FindCorrections(string key)
		{
			var corrections = ExpandQuery(key);
			return corrections.SelectMany(c => Find(c)).Distinct()
				.Select(c => (
					word: c, 
					editDistance: _wordDistanceCalculator.ByDamerauLevenshtein(
						(key, key.GetKeyCodes(_defaultLanguage.KeyCodes, _otherlanguages.Select(l => l.KeyCodes).ToArray()) ),
						(c, c.GetKeyCodes(_defaultLanguage.KeyCodes, _otherlanguages.Select(l => l.KeyCodes).ToArray())), true))).OrderBy(c => c.editDistance).Select(c => c.word)
				.ToArray();
		}

		internal protected override WordCorrection[] FindCorrectionsInfo(string key)
		{
			return FindCorrections(key).Select(w => new WordCorrection { Correction = w, Info = _strict.Get(w) }).Where(c => c.Info != null).ToArray();
		}

		protected internal override async Task<WordCorrection[]> FindCorrectionsInfoAsync(string key, CancellationToken token)
		{
			return (await Task.Factory.StartNew(() =>
			{
				var corrections = ExpandQuery(key);
				return corrections.SelectMany(c => token.IsCancellationRequested ? new string[0] : Find(c)).Distinct()
					.Select(c => (
					word: c,
					editDistance: _wordDistanceCalculator.ByDamerauLevenshtein(
						(key, key.GetKeyCodes(_defaultLanguage.KeyCodes, _otherlanguages.Select(l => l.KeyCodes).ToArray())),
						(c, c.GetKeyCodes(_defaultLanguage.KeyCodes, _otherlanguages.Select(l => l.KeyCodes).ToArray())), true))).OrderBy(c => c.editDistance).Select(c => c.word)
					.ToArray();
			}, token)).Select(w => new WordCorrection { Correction = w, Info = _strict.Get(w) }).Where(c => c.Info != null).ToArray();
		}
		#endregion

		#region Private Methods
		private async Task BuildNGrams(string[] keys)
		{
			await Task.Factory.StartNew(() =>
			{
				_ngramms = new Dictionary<string, Dictionary<int, string[]>>[keys.Max(k => k.Length) - _N + 1];
				for (var i = 0; i < keys.Length; i++)
				{
					var ngramms = GetNGramms(keys[i]);
					for (var j = 0; j < ngramms.Length; j++)
					{
						if (_ngramms[j] == null)
						{
							_ngramms[j] = new Dictionary<string, Dictionary<int, string[]>>();
						}
						if (_ngramms[j].TryGetValue(ngramms[j], out Dictionary<int, string[]> ngrammsByLength))
						{
							if (ngrammsByLength.TryGetValue(keys[i].Length, out string[] words))
							{
								ngrammsByLength[keys[i].Length] = words.Add(keys[i]);
							}
							else
							{
								ngrammsByLength.Add(keys[i].Length, new[] { keys[i] });
							}
						}
						else
						{
							_ngramms[j].Add(ngramms[j], new Dictionary<int, string[]> { { keys[i].Length, new[] { keys[i] } } });
						}
					}
				}
			});
		}

		private string[] Find(string key)
		{
			var ngramms = GetNGramms(key);
			var corrections = new string[0];
			var editDistance = _editDistance;
			for (var i = 0; i < ngramms.Length; i++)
			{
				for (var j = i - editDistance; j < i + editDistance + 1; j++)
				{
					if (j >= 0 && j < _ngramms.Length && _ngramms[j].TryGetValue(ngramms[i], out Dictionary<int, string[]> wordsByLength))
					{
						for (var k = key.Length - editDistance; k < key.Length + editDistance + 1; k++)
						{
							if (wordsByLength.TryGetValue(k, out string[] words))
							{
								corrections = corrections.Concat(words).Distinct().ToArray();
							}
						}
					}
				}
			}
			return corrections;
		}

		private string[] GetNGramms(string key)
		{
			var ngramms = new string[0];
			for (var i = 0; i < key.Length - _N + 1; i++)
			{
				ngramms = ngramms.Add(key.Substring(i, _N));
			}
			return ngramms;
		}
		#endregion
	}
}
