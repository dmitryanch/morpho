using Core.Ext;
using Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fuzzy
{
	public abstract class EngineBase : IFuzzy
	{
		#region Protected Fields
		internal protected IStrict _strict;
		internal protected ILanguageDataProvider _langDataProvider;
		internal protected int _editDistance;
		internal protected bool _transliterate;
		internal protected bool _convertByKeycodes;
		#endregion

		#region Public API
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

		#region Abstract Methods
		public abstract void InitFuzzy(IStrict strictEngine, ILanguageDataProvider langDataProvider, object settings);
		internal protected abstract string[] FindCorrections(string key);
		internal protected abstract (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] FindCorrectionsInfo(string key);
		internal protected abstract Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> FindCorrectionsInfoAsync(string key, CancellationToken token);
		#endregion

		#region Protected Methods
		internal protected virtual void InitFuzzyBase(IStrict strictEngine, ILanguageDataProvider langDataProvider, (int EditDistance, bool Transliterate, bool ConvertByKeycodes) settings)
		{
			_strict = strictEngine;
			_langDataProvider = langDataProvider;
			_editDistance = settings.EditDistance;
			_transliterate = settings.Transliterate;
			_convertByKeycodes = settings.ConvertByKeycodes;
		}

		internal protected (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] GetExact(string key)
		{
			var exact = _strict.Get(key);
			return exact != null ? new[] { (Correction: key, Info: exact) } : null;
		}

		internal protected string[] TransliterateFromEn(string key)
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

		internal protected string[] ConvertByKeycodes(string key)
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
