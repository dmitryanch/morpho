using Core.Classes;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IStrict
	{
		Task Init();
		bool Contains(string key);
		string[] Lemmatize(string key);
		WordInfo[] Get(string key);
		WordEntry GetWithCodes(string key);
		string[] GetKeys();
		string LanguageTitle { get; }
	}
}
