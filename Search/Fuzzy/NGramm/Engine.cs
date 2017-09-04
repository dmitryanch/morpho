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
		public (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get(string key)
		{
			throw new NotImplementedException();
		}

		public (string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[] Get(string key, bool exactEnough)
		{
			throw new NotImplementedException();
		}

		public Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync(string key, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task<(string Correction, (IMorphoSigns[] Signs, string Lemma)[] Info)[]> GetAsync(string key, bool exactEnough, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public string[] GetCorrections(string key)
		{
			throw new NotImplementedException();
		}

		public void InitFuzzy(IStrict strictEngine, ILanguageDataProvider langDataProvider, (int EditDistance, bool Translite, bool ConvertByKeycodes, bool UseShortAlphabet) settings)
		{
			throw new NotImplementedException();
		}
	}
}
