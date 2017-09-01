using Core.Ext;
using Core.Interfaces;
using Core.Lang.RU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RU.OpenCorpora
{
	public class OpenCorporaDataProvider : IDictionaryDataProvider, IDisposable
	{
		private const string OPENCORPORA_DICTIONARY_FILEPATH = "C:/Users/dexp/Documents/Semantix/dict.opcorpora.txt/dict.txt";
		private readonly Dictionary<string, uint> _grammems = new Dictionary<string, uint>();
		private readonly Dictionary<uint, string> _descriptions = new Dictionary<uint, string>();
		private readonly Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> _dictionary =
			new Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)>();
		private ILanguageDataProvider _langDataProvider;

		public void Init(ILanguageDataProvider langDataProvider)
		{
			_langDataProvider = langDataProvider;
			var i = 1;
			var keys = OpenCorporaGrammems.StringsDescriptions.Keys.ToList();
			foreach (var s in keys)
			{
				var primes = (uint)i++;
				_grammems.Add(s, primes);
				_descriptions.Add(primes, OpenCorporaGrammems.StringsDescriptions[s]);
			}
		}

		public Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> Read()
		{
			OnReading?.Invoke(this, null);
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
					ProcessArticle(article);
				}
			}
			OnRead?.Invoke(this, null);
			return _dictionary;
		}

		public bool Test(IStrict searcher)
		{
			OnTesting?.Invoke(this, null);
			var correct = true;
			using (var stream = new FileStream(OPENCORPORA_DICTIONARY_FILEPATH, FileMode.Open))
			using (var sr = new StreamReader(stream))
			{
				var getLemma = true;
				var lemma = (string)null;
				while (!sr.EndOfStream)
				{
					var row = sr.ReadLine();
					var strings = row.Split(new[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if(getLemma)
					{
						lemma = strings[0];
					}
					if (strings.Length == 1)
					{
						getLemma = true;
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
		}

		public EventHandler<EventArgs> OnReading { get; set; }
		public EventHandler<EventArgs> OnRead { get; set; }
		public EventHandler<EventArgs> OnTesting { get; set; }
		public EventHandler<EventArgs> OnTested { get; set; }

		private void ProcessArticle(List<string[]> article)
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
				if (_dictionary.TryGetValue(wordForm[0], out ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes) entry))
				{
					var word = entry.Words.FirstOrDefault(w => string.Equals(w.Lemma, lemma));
					var containsSigns = false;
					if (word.Lemma == null)
					{
						word = (Signs: new[] { morphoSigns }, Lemma: lemma);
						entry.Words.Add(word);
						containsSigns = true;
					}
					var signs = word.Signs;
					if (!containsSigns && !signs.Contains(morphoSigns))
					{
						signs.Add(morphoSigns);
					}
				}
				else
				{
					entry = (Words: new[] { (Signs: new[] { morphoSigns }, Lemma: lemma) }, Codes: wordForm[0].GetKeyCodes(_langDataProvider.KeyCodes));
					_dictionary[wordForm[0]] = entry;
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

		#region IDisposable Support
		private bool disposedValue = false; // Для определения избыточных вызовов

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_grammems.Clear();
				_descriptions.Clear();
				_dictionary.Clear();

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
