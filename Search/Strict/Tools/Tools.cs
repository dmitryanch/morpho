﻿using Core.Classes;
using Core.Ext;
using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.MPHF;

namespace Strict.Tools
{
	internal static class Tools
	{
		public static (byte[][] Keys, WordInfo[][] Lemmas, byte[][] KeyCodes, MinPerfectHashFunction Mphf)
			ParseData(Dictionary<string, WordEntry> dict)
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
			var Lemmas = new WordInfo[mphf.N][];
			var keyCodes = new byte[mphf.N][];
			var keyBytes = new byte[mphf.N][];
			foreach (var pair in dict)
			{
				var wordBytes = Encoding.UTF8.GetBytes(pair.Key);
				var index = (int)mphf.Search(wordBytes);
				var wordEntries = pair.Value.Words.Select(w => new WordInfo { Signs = signs[w.Signs.ComputeHash()], Lemma = lemmas[w.Lemma] }).ToArray();
				keyBytes[index] = wordBytes;
				Lemmas[index] = wordEntries;
				keyCodes[index] = bytes[pair.Value.Codes.ComputeHash()];
			}
			return (Keys: keyBytes, Lemmas: Lemmas, KeyCodes: keyCodes, Mphf: mphf);
		}
	}
}
