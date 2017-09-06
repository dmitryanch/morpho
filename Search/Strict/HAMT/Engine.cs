using Core.Interfaces;
using Strict.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.MPHF;
using Utils.HAMT;

namespace Search.Strict.HAMT
{
	public sealed class Engine : IStrict
	{
		#region Private Fields
		private MinPerfectHashFunction _mphf;
		private byte[][] _keyCodes;
		private static readonly Hamt<string, (IMorphoSigns[] Signs, string Lemma)[]> _hamt =
			new Hamt<string, (IMorphoSigns[] Signs, string Lemma)[]>();
		#endregion

		#region Public API
		public void Init(Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> dict)
		{
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

		public (IMorphoSigns[] Signs, string Lemma)[] Get(string key)
		{
			return _hamt.Get(key)?.Value;
		}

		public ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes) GetWithCodes(string key)
		{
			return (Words: _hamt.Get(key)?.Value, Codes: _keyCodes[(int)_mphf.Search(Encoding.UTF8.GetBytes(key))]);
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
