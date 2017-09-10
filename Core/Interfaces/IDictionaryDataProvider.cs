using Core.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IDictionaryDataProvider
	{
		Task Init();
		Task<Dictionary<string, WordEntry>> GetData();
		Task<bool> Test(IStrict searcher);
		EventHandler<EventArgs> OnReading { get; set; }
		EventHandler<EventArgs> OnRead { get; set; }
		EventHandler<EventArgs> OnTesting { get; set; }
		EventHandler<EventArgs> OnTested { get; set; }
		string LanguageTitle { get; }
	}
}
