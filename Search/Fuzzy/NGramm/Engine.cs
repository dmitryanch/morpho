using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fuzzy.NGramm
{
	public class Engine : IFuzzy
	{
		public KeyValuePair<string, IMorphoSigns[]>[] Get(string key)
		{
			throw new NotImplementedException();
		}

		public KeyValuePair<string, IMorphoSigns[]>[] Get(string key, bool exactEnough)
		{
			throw new NotImplementedException();
		}

		public Task<KeyValuePair<string, IMorphoSigns[]>[]> GetAsync(string key, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task<KeyValuePair<string, IMorphoSigns[]>[]> GetAsync(string key, bool exactEnough, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public void InitFuzzy(IStrict strictEngine)
		{
			throw new NotImplementedException();
		}
	}
}
