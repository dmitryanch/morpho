using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.MPHF;

namespace Strict.Tools
{
	internal static class Tools
	{
		#region Extension Methods
		public static ulong ComputeHash<T>(this T[] array)
		{
			if (array == null) return 0;
			unchecked
			{
				ulong hash = 17;
				for (var i = 0; i < array.Length; i++)
					hash = 31 * hash + (ulong)array[i].GetHashCode();
				return hash;
			}
		}
		#endregion

		public static ((IMorphoSigns[] Signs, string Lemma)[][] Lemmas, byte[][] KeyCodes, MinPerfectHashFunction Mphf)
			ParseData(Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> dict)
		{
			var keys = dict.Keys.ToArray();
			var signs = new Dictionary<ulong, IMorphoSigns[]>();
			var lemmas = new Dictionary<string, string>();
			var bytes = new Dictionary<ulong, byte[]>();
			foreach (var kvp in dict)
			{
				foreach (var word in kvp.Value.Words)
				{
					var signsHash = word.Signs.ComputeHash();
					if (!signs.ContainsKey(signsHash))
					{
						signs.Add(signsHash, word.Signs);
					}
					if (!lemmas.ContainsKey(word.Lemma))
					{
						lemmas.Add(word.Lemma, word.Lemma);
					}
				}
				var bytesHash = kvp.Value.Codes.ComputeHash();
				if (!bytes.ContainsKey(bytesHash))
				{
					bytes.Add(bytesHash, kvp.Value.Codes);
				}
			}
			var keygen = new KeyGenerator(keys);
			var mphf = MinPerfectHashFunction.Create(keygen, 1);
			var Lemmas = new(IMorphoSigns[] Signs, string Lemma)[mphf.N][];
			var keyCodes = new byte[mphf.N][];
			foreach (var pair in dict)
			{
				var wordBytes = Encoding.UTF8.GetBytes(pair.Key);
				var index = (int)mphf.Search(wordBytes);
				var wordEntries = pair.Value.Words.Select(w => (Signs: signs[w.Signs.ComputeHash()], Lemma: lemmas[w.Lemma])).ToArray();
				Lemmas[index] = wordEntries;
				keyCodes[index] = bytes[pair.Value.Codes.ComputeHash()];
			}
			return (Lemmas: Lemmas, KeyCodes: keyCodes, Mphf: mphf);
		}
	}
}
