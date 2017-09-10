using Core.Interfaces;
using Strict.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.MPHF;
using Utils.HAMT;
using Core.Classes;
using System.Threading.Tasks;

namespace Search.Strict.HAMT
{
	public sealed class Engine : IStrict
	{
		#region Private Fields
		private IDictionaryDataProvider _corpora;
		private MinPerfectHashFunction _mphf;
		private byte[][] _keyCodes;
		private static readonly Hamt<string, WordInfo[]> _hamt =
			new Hamt<string, WordInfo[]>();
		#endregion

		#region Constructor
		public Engine(IDictionaryDataProvider corpora)
		{
			_corpora = corpora;
		}
		#endregion

		#region Public API
		public string LanguageTitle => _corpora.LanguageTitle;

		public async Task Init()
		{
			await _corpora.Init();
			var dict = await _corpora.GetData();
			var arrays = Tools.ParseData(dict);
			_mphf = arrays.Mphf;
			_keyCodes = arrays.KeyCodes;
			var lemmas = arrays.Lemmas;

			_hamt.SetHashEvaluator(s => (int)_mphf.Search(Encoding.UTF8.GetBytes(s)));
			foreach (var pair in dict)
			{
				_hamt.Add(pair.Key, lemmas[(int)_mphf.Search(Encoding.UTF8.GetBytes(pair.Key))]);
			}
		}

		public string[] Lemmatize(string key)
		{
			return _hamt.Get(key)?.Value.Select(w => w.Lemma).ToArray();
		}

		public WordInfo[] Get(string key)
		{
			return _hamt.Get(key)?.Value;
		}

		public WordEntry GetWithCodes(string key)
		{
			return new WordEntry { Words = _hamt.Get(key)?.Value, Codes = _keyCodes[(int)_mphf.Search(Encoding.UTF8.GetBytes(key))] };
		}

		public bool Contains(string key)
		{
			return _hamt.Get(key) != null;
		}

		public string[] GetKeys()
		{
			return _hamt.GetKeys().ToArray();
		}
		#endregion
	}
}
