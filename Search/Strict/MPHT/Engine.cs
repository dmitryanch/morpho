using System.Collections.Generic;
using System.Text;
using Core.Interfaces;
using Utils.MPHF;
using System.Linq;
using Strict.Tools;
using Core.Ext;
using Core.Classes;
using System.Threading.Tasks;

namespace Search.Strict.MPHT
{
	public sealed class Engine : IStrict
	{
		#region Private Fields
		private IDictionaryDataProvider _corpora;
		private MinPerfectHashFunction _mphf;
		private byte[][] _keyCodes;
		private WordInfo[][] _lemmas;
		private byte[][] _keys;
		#endregion

		#region Constructor
		public Engine(IDictionaryDataProvider corpora)
		{
			_corpora = corpora;
		}
		#endregion

		#region Public API
		public string LanguageTitle => _corpora.LanguageTitle;

		public string[] Lemmatize(string key)
		{
			var index = GetIndex(key);
			return index > -1 ? _lemmas[index].Select(l => l.Lemma).ToArray() : null;
		}

		public WordInfo[] Get(string key)
		{
			var index = GetIndex(key);
			return index > -1 ? _lemmas[index] : null;
		}

		public WordEntry GetWithCodes(string key)
		{
			var index = GetIndex(key);
			return index > -1 ? new WordEntry { Words = _lemmas[index], Codes = _keyCodes[index] } : null;
		}

		public async Task Init()
		{
			await _corpora.Init();
			(_keys, _lemmas, _keyCodes, _mphf) = Tools.ParseData(await _corpora.GetData());
		}

		public bool Contains(string key)
		{
			var index = GetIndex(key);
			return index > -1;
		}

		public string[] GetKeys()
		{
			return _keys.Select(b => b != null ? Encoding.UTF8.GetString(b) : null).Where(s => s != null).ToArray();
		}
		#endregion

		#region Private Methods
		private int GetIndex(string key)
		{
			var wordBytes = Encoding.UTF8.GetBytes(key);
			var index = _mphf.Search(wordBytes);
			return Contains(index, wordBytes) ? (int)index : -1;
		}
		private bool Contains(uint index, byte[] key)
		{
			var word = _keys[index];
			return word != null && ArrayExt.Equals(word, key);
		}
		#endregion
	}
}
