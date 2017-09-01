using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IStrict
    {
		void Init(Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> keys);
		string[] Lemmatize(string key);
		(IMorphoSigns[] Signs, string Lemma)[] Get(string key);
		((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes) GetWithCodes(string key);
	}
}
