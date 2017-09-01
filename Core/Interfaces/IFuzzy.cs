using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IFuzzy
	{
		void InitFuzzy(IStrict strictEngine);
		KeyValuePair<string, IMorphoSigns[]>[] Get(string key);
		KeyValuePair<string, IMorphoSigns[]>[] Get(string key, bool exactEnough);
		Task<KeyValuePair<string, IMorphoSigns[]>[]> GetAsync(string key, CancellationToken token);
		Task<KeyValuePair<string, IMorphoSigns[]>[]> GetAsync(string key, bool exactEnough, CancellationToken token);
	}
}
