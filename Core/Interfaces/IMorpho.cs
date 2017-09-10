using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IMorpho
	{
		IFuzzy Fuzzy { get; }
		Task Init(object fuzzySettings);
		string[] Get(string key);
	}
}
