using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Core.Classes
{
	public class LanguageDataProvider : ILanguageDataProvider
	{
		#region Private Fields
		private Dictionary<string, ILanguageData> _languages;
		#endregion

		#region Public API
		public ILanguageDataProvider Add(ILanguageData language)
		{
			if (language == null)
			{
				return this;
			}
			if (_languages == null)
			{
				_languages = new Dictionary<string, ILanguageData>();
			}
			_languages.Add(language.Title.ToUpper(), language);
			return this;
		}

		public ILanguageData Get(string languageTitle)
		{
			return _languages != null && _languages.TryGetValue(languageTitle.ToUpper(), out ILanguageData language) ? language : null;
		}

		public ILanguageData[] GetAll()
		{
			return _languages?.Values.ToArray();
		}

		public ILanguageData[] GetAllExcept(params ILanguageData[] languages)
		{
			return _languages?.Values.Except(languages).ToArray();
		}

		public ILanguageData[] GetAllExcept(params string[] languages)
		{
			return _languages?.Values.Where(l => !languages.Contains(l.Title)).ToArray();
		}
		#endregion
	}
}
