using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IFuzzy
	{
		void InitFuzzy(IStrict strictEngine, ILanguageDataProvider langDataProvider, object settings);
		string[] GetCorrections(string key);
		(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get(string key);
		//(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get((char c, int interval)[] key);
		(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get(string key, bool exactEnough);
		//(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get((char c, int interval)[] key, bool exactEnough);
		Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync(string key, CancellationToken token);
		//Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync((char c, int interval)[] key, CancellationToken token);
		Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync(string key, bool exactEnough, CancellationToken token);
		//Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync((char c, int interval)[] key, bool exactEnough, CancellationToken token);
	}
}
