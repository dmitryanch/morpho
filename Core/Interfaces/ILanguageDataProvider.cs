namespace Core.Interfaces
{
	public interface ILanguageDataProvider
	{
		ILanguageDataProvider Add(ILanguageData language);
		ILanguageData Get(string languageTitle);
		ILanguageData[] GetAll();
		ILanguageData[] GetAllExcept(params ILanguageData[] language);
		ILanguageData[] GetAllExcept(params string[] language);
	}
}
