using System.Collections.Generic;
using System.Text;
using Core.Interfaces;
using Utils.MPHF;
using System.Linq;
using Strict.Tools;
using Core.Ext;

namespace Search.Strict.MPHT
{
	public sealed class Engine : IStrict
	{
		#region Private Fields
		private MinPerfectHashFunction _mphf;
		private byte[][] _keyCodes;
		private (IMorphoSigns[] Signs, string Lemma)[][] _lemmas;
		private byte[][] _keys;
		#endregion

		#region Public API
		public string[] Lemmatize(string key)
		{
			var index = GetIndex(key);
			return index > -1 ? _lemmas[index].Select(l => l.Lemma).ToArray() : null;
		}

		public (IMorphoSigns[] Signs, string Lemma)[] Get(string key)
		{
			var index = GetIndex(key);
			return index > -1 ? _lemmas[index] : null;
		}

		public ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes) GetWithCodes(string key)
		{
			var index = GetIndex(key);
			return index > -1 ? (Words: _lemmas[index], Codes: _keyCodes[index]) : default(((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes));
		}

		public void Init(Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> dict)
		{
			(_keys, _lemmas, _keyCodes, _mphf) = Tools.ParseData(dict);
		}

		public bool Contains(string key)
		{
			var index = GetIndex(key);
			return index > -1;
		}

		public string[] GetKeys()
		{
			return _keys.Select(b => Encoding.UTF8.GetString(b)).ToArray();
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
			return ArrayExt.Equals(word, key);
		}
		#endregion
	}
}
