using Core.Classes;
using Core.Ext;
using Core.Interfaces;
using Core.Lang.RU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RU.OpenCorpora
{
	public class OpenCorporaDataProvider : IDictionaryDataProvider, IDisposable
	{
		#region Private Fields
		private const string OPENCORPORA_DICTIONARY_FILEPATH = "C:/Users/dexp/Documents/Semantix/dict.opcorpora.txt/dict.txt";
		private const string LANGUAGE_TITLE = "RU";
		private readonly Dictionary<string, uint> _grammems = new Dictionary<string, uint>();
		private readonly Dictionary<uint, string> _descriptions = new Dictionary<uint, string>();
		private ILanguageData _languageData;
		#endregion

		#region Constructor
		public OpenCorporaDataProvider(ILanguageDataProvider langDataProvider)
		{
			_languageData = langDataProvider.Get(LANGUAGE_TITLE);
		}
		#endregion

		#region Public API
		public Task Init()
		{
			var i = 1;
			var keys = OpenCorporaGrammems.StringsDescriptions.Keys.ToList();
			foreach (var s in keys)
			{
				var primes = (uint)i++;
				_grammems.Add(s, primes);
				_descriptions.Add(primes, OpenCorporaGrammems.StringsDescriptions[s]);
			}
			return Task.CompletedTask;
		}

		public async Task<Dictionary<string, WordEntry>> GetData()
		{
			return await Task.Factory.StartNew(() =>
				{
					OnReading?.Invoke(this, null);
					var dictionary = new Dictionary<string, WordEntry>();
					using (var stream = new FileStream(OPENCORPORA_DICTIONARY_FILEPATH, FileMode.Open))
					using (var sr = new StreamReader(stream))
					{
						var article = new List<string[]>();
						var separators = new[] { ' ', ',', '\t' };
						while (!sr.EndOfStream)
						{
							var row = sr.ReadLine();
							if (row == null) continue;
							var strings = row.Split(separators, StringSplitOptions.RemoveEmptyEntries);
							if (strings.Length == 1)
							{
								article.Clear();
								continue;
							}
							if (strings.Any())
							{
								article.Add(strings);
								continue;
							}
							ProcessArticle(article, ref dictionary);
						}
					}
					OnRead?.Invoke(this, null);
					return dictionary;
				});
		}

		public async Task<bool> Test(IStrict searcher)
		{
			return await Task.Factory.StartNew(() =>
				{
					OnTesting?.Invoke(this, null);
					var correct = true;
					using (var stream = new FileStream(OPENCORPORA_DICTIONARY_FILEPATH, FileMode.Open))
					using (var sr = new StreamReader(stream))
					{
						var getLemma = false;
						var lemma = (string)null;
						while (!sr.EndOfStream)
						{
							var row = sr.ReadLine();
							var strings = row.Split(new[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
							if (getLemma)
							{
								lemma = strings[0];
								getLemma = false;
							}
							if (strings.Length <= 1)
							{
								getLemma = strings.Length == 1;
								continue;
							}
							correct &= StrictlyCheck(searcher, strings, lemma);
							if (!correct)
							{
								break;
							}
						}
					}
					OnTested?.Invoke(this, null);
					return correct;
				});
		}

		public EventHandler<EventArgs> OnReading { get; set; }

		public EventHandler<EventArgs> OnRead { get; set; }

		public EventHandler<EventArgs> OnTesting { get; set; }

		public EventHandler<EventArgs> OnTested { get; set; }

		public string LanguageTitle => LANGUAGE_TITLE;
		#endregion

		#region Private Methods
		private void ProcessArticle(List<string[]> article, ref Dictionary<string, WordEntry> dictionary)
		{
			if (article.All(r => r.Contains("Fixd")) && article.Count > 1 &&
				article.Select(r => r[0]).Distinct().Count() == 1)
			{
				var newStrings = new List<string>() { article[0][0] };
				for (var i = 1; i < article[0].Length; i++)
				{
					var distinct =
						article.Select(r => i < r.Length ? r[i] : null)
							.Where(s => s != null)
							.Distinct()
							.ToList();
					if (distinct.Count == 1)
					{
						newStrings.Add(distinct[0]);
					}
				}
				article.Clear();
				article.Add(newStrings.ToArray());
			}
			var lemma = article[0][0];
			foreach (var wordForm in article)
			{
				var morphoSigns = ParseMorphoSigns(wordForm);
				if (dictionary.TryGetValue(wordForm[0], out WordEntry entry))
				{
					var word = entry.Words.FirstOrDefault(w => string.Equals(w.Lemma, lemma));
					var containsSigns = false;
					if (word == null)
					{
						word = new WordInfo { Signs = new[] { morphoSigns }, Lemma = lemma };
						entry.Words = entry.Words.Add(word);
						containsSigns = true;
					}
					var signs = word.Signs;
					if (!containsSigns && !signs.Contains(morphoSigns))
					{
						word.Signs = signs.Add(morphoSigns);
					}
				}
				else
				{
					entry = new WordEntry
					{
						Words = new[] { new WordInfo { Signs = new[] { morphoSigns }, Lemma = lemma } },
						Codes = wordForm[0].GetKeyCodes(_languageData.KeyCodes)
					};
					dictionary[wordForm[0].ToLower()] = entry;
				}
			}
		}

		private IMorphoSigns ParseMorphoSigns(string[] stringSigns)
		{
			var signs = new RuMorphoSigns();
			for (var i = 1; i < stringSigns.Length; i++)
			{
				var multyplier = (byte)_grammems[stringSigns[i]];
				if (multyplier > 255 || signs.Contains(multyplier))
				{
					throw new InvalidOperationException();
				}
				signs.Add(multyplier);
			}
			return signs;
		}

		public bool StrictlyCheck(IStrict searcher, string[] wordForm, string lemma)
		{
			var entry = searcher.Get(wordForm[0]);
			var signs = entry?.SelectMany(w => w.Signs);
			if (entry == null || !entry.Select(w => w.Lemma).Contains(lemma) || signs == null)
			{
				return false;
			}
			var processedString = ParseMorphoSigns(wordForm);
			return !(wordForm.Length > 1 && (!signs.Contains(processedString) && !wordForm.Contains("Fixd")));
		}
		#endregion

		#region IDisposable Support
		private bool disposedValue = false; // Для определения избыточных вызовов

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_grammems.Clear();
				_descriptions.Clear();

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
