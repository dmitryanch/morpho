using Core.Interfaces;
using System.Threading.Tasks;

namespace MorphoProcessor
{
	public sealed class Morpho : IMorpho
	{
		#region Private Fields
		IFuzzy _fuzzy;
		#endregion

		public Morpho(IFuzzy fuzzy)
		{
			_fuzzy = fuzzy;
		}

		#region Public API
		public IFuzzy Fuzzy => _fuzzy;

		public string[] Get(string key)
		{
			if (!Validate(key))
			{
				return null;
			}
			return _fuzzy.GetCorrections(key.ToLower());
		}

		public async Task Init(object fuzzySettings)
		{
			await _fuzzy.Init(fuzzySettings);
		}
		#endregion

		#region Private Methods
		private bool Validate(string key)
		{
			return !string.IsNullOrEmpty(key) && key.Length < 101;
		}
		private string DefineLanguage(string key)
		{
			throw new System.Exception();
		}
		#endregion
	}
}
