using Core.Classes;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IFuzzy
	{
		Task Init(object settings);
		string[] GetCorrections(string key);
		WordCorrection[] Get(string key);
		WordCorrection[] Get(string key, bool exactEnough);
		Task<WordCorrection[]> GetAsync(string key, CancellationToken token);
		Task<WordCorrection[]> GetAsync(string key, bool exactEnough, CancellationToken token);
	}
}
