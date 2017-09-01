using System;
using System.Collections.Generic;
using System.Text;
using Core.Interfaces;
using Utils.MPHF;
using System.Linq;
using Strict.Tools;

namespace Search.Strict.MPHT
{
	public sealed class Engine : IStrict
	{
		private MinPerfectHashFunction _mphf;
		private byte[][] _keyCodes;
		private (IMorphoSigns[] Signs, string Lemma)[][] _lemmas;

		public string[] Lemmatize(string key)
		{
			var wordBytes = Encoding.UTF8.GetBytes(key);
			var index = (int)_mphf.Search(wordBytes);
			return _lemmas[index].Select(l => l.Lemma).ToArray();
		}

		public (IMorphoSigns[] Signs, string Lemma)[] Get(string key)
		{
			var wordBytes = Encoding.UTF8.GetBytes(key);
			var index = (int)_mphf.Search(wordBytes);
			return _lemmas[index];
		}

		public ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes) GetWithCodes(string key)
		{
			var wordBytes = Encoding.UTF8.GetBytes(key);
			var index = (int)_mphf.Search(wordBytes);
			return (Words: _lemmas[index], Codes: _keyCodes[index]);
		}

		public void Init(Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> dict)
		{
			(_lemmas, _keyCodes, _mphf) = Tools.ParseData(dict);
		}
	}
}
