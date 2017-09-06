using System.Collections.Generic;

namespace Core.Interfaces
{
	public interface IStrict
    {
		void Init(Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> keys);
		bool Contains(string key);
		string[] Lemmatize(string key);
		(IMorphoSigns[] Signs, string Lemma)[] Get(string key);
		((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes) GetWithCodes(string key);
		string[] GetKeys();
	}
}
