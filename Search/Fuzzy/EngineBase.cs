using Core.Classes;
using Core.Ext;
using Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Search.Fuzzy
{
	public abstract class EngineBase : IFuzzy
	{
		#region Protected Fields
		internal protected IStrict _strict;
		internal protected ILanguageData _defaultLanguage;
		internal protected ILanguageData[] _otherlanguages;
		internal protected int _editDistance;
		internal protected bool _transliterate;
		internal protected bool _convertByKeycodes;
		#endregion

		#region Constructor
		public EngineBase(IStrict strictEngine, ILanguageDataProvider langDataProvider)
		{
			_strict = strictEngine;
			_defaultLanguage = langDataProvider.Get(_strict.LanguageTitle);
			_otherlanguages = langDataProvider.GetAllExcept(_defaultLanguage);
		}
		#endregion

		#region Public API
		public string[] GetCorrections(string key)
		{
			return FindCorrections(key);
		}

		public WordCorrection[] Get(string key)
		{
			return FindCorrectionsInfo(key);
		}

		public WordCorrection[] Get(string key, bool exactEnough)
		{
			var corrections = GetExact(key);
			if (corrections != null && exactEnough)
			{
				return corrections;
			}
			return FindCorrectionsInfo(key);
		}

		public async Task<WordCorrection[]> GetAsync(string key, CancellationToken token)
		{
			return await FindCorrectionsInfoAsync(key, token);
		}

		public async Task<WordCorrection[]> GetAsync(string key, bool exactEnough, CancellationToken token)
		{
			var corrections = GetExact(key);
			if (corrections != null && exactEnough)
			{
				return corrections;
			}
			return await FindCorrectionsInfoAsync(key, token);
		}
		#endregion

		#region Abstract Methods
		public abstract Task Init(object settings);
		internal protected abstract string[] FindCorrections(string key);
		internal protected abstract WordCorrection[] FindCorrectionsInfo(string key);
		internal protected abstract Task<WordCorrection[]> FindCorrectionsInfoAsync(string key, CancellationToken token);
		#endregion

		#region Protected Methods
		internal protected virtual async Task InitFuzzyBase((int EditDistance, bool Transliterate, bool ConvertByKeycodes) settings)
		{
			await _strict.Init();
			_editDistance = settings.EditDistance;
			_transliterate = settings.Transliterate;
			_convertByKeycodes = settings.ConvertByKeycodes;
		}

		internal protected WordCorrection[] GetExact(string key)
		{
			var exact = _strict.Get(key);
			return exact != null ? new[] { new WordCorrection { Correction = key, Info = exact } } : null;
		}

		internal protected string[] TransliterateFromEn(string key)
		{
			if (_defaultLanguage.TranslitFromEn == null) return null;
			var corrections = new[] { key };
			foreach (var item in _defaultLanguage.TranslitFromEn)
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

		internal protected string[] ConvertByKeycodes(string key)
		{
			var wordKeycodes = key.GetKeyCodes(_defaultLanguage.KeyCodes, _otherlanguages.Select(l => l.KeyCodes).ToArray());
			var words = new string[0];
			for (var i = 0; i < wordKeycodes.Length; i++)
			{
				var tempWords = new string[0];
				var chars = _defaultLanguage.CharsByKeycode[wordKeycodes[i]];
				words = chars.SelectMany(c => words.Any() ? words.Select(w => w.Insert(w.Length, c.ToString())) : new[] { c.ToString() }).ToArray();
			}
			return words;
		}

		internal protected string[] ExpandQuery(string key)
		{
			var corrections = new[] { key };
			if (_transliterate)
			{
				corrections = corrections.Concat(TransliterateFromEn(key)).Distinct().ToArray();
			}
			if (_convertByKeycodes)
			{
				corrections = corrections.Concat(ConvertByKeycodes(key)).Distinct().ToArray();
			}
			return corrections;
		}
		#endregion
	}
}
