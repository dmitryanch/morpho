using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
	public interface IDictionaryDataProvider
    {
		void Init(ILanguageDataProvider langDataProvider);
		Dictionary<string, ((IMorphoSigns[] Signs, string Lemma)[] Words, byte[] Codes)> Read();
		bool Test(IStrict searcher);
		EventHandler<EventArgs> OnReading { get; set; }
		EventHandler<EventArgs> OnRead { get; set; }
		EventHandler<EventArgs> OnTesting { get; set; }
		EventHandler<EventArgs> OnTested { get; set; }
	}
}
