using Core.Classes;
using Core.Interfaces;
using EN;
using Microsoft.Extensions.DependencyInjection;
using RU;
using RU.OpenCorpora;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Morpho.Test
{
	internal class EntryPoint
	{
		private static IServiceProvider _services;
		static void Main(string[] args)
		{
			Configure();

			//TestStrictEngineCorpora().Wait();
			TestMorpho().Wait();

			Console.ReadLine();
		}

		private static void Configure()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IDictionaryDataProvider, OpenCorporaDataProvider>();
			services.AddSingleton<ILanguageDataProvider, LanguageDataProvider>();
			//services.AddSingleton<IStrict, Search.Strict.MPHT.Engine>();
			services.AddSingleton<IStrict, Search.Strict.HAMT.Engine>();
			//services.AddSingleton<IFuzzy, Search.Fuzzy.Expand.Engine>();
			services.AddSingleton<IFuzzy, Search.Fuzzy.NGramm.Engine>();
			services.AddSingleton<IMorpho, MorphoProcessor.Morpho>();
			_services = services.BuildServiceProvider();
			var languages = _services.GetRequiredService<ILanguageDataProvider>();
			languages.Add(new RuLanguageData()).Add(new EnLanguageData());
		}

		private async static Task TestStrictEngineCorpora()
		{
			Action prepare = () =>
			{
				var corpora = _services.GetRequiredService<IDictionaryDataProvider>();
				var strict = _services.GetRequiredService<IStrict>();
				strict.Init();
			};
			Func<Task<bool>> test = async () => await _services.GetRequiredService<IDictionaryDataProvider>().Test(_services.GetRequiredService<IStrict>());

			prepare();
			var start = DateTime.Now;
			Console.WriteLine($"Test {(await test() ? "passed successefully!" : "failed.")}");
			Console.WriteLine($"Testing time is about {(DateTime.Now - start).TotalMilliseconds} ms.");
			Console.WriteLine($"Total Memory is {GC.GetTotalMemory(true) / 1024 / 1024} Mb.");
		}

		private async static Task TestMorpho()
		{
			Func<Task> prepare = async () =>
			{
				var settings = _services.GetRequiredService<IFuzzy>() is Search.Fuzzy.Expand.Engine 
					? (object) (EditDistance: 2, Transliterate: true, ConvertByKeycodes: true, UseShortAlphabet: true) 
					: (EditDistance: 2, Transliterate: true, ConvertByKeycodes: true, N: 3);
				var start = DateTime.Now;
				await _services.GetRequiredService<IMorpho>().Init(settings);
				Console.WriteLine($"Initialization time is about {(DateTime.Now - start).TotalMilliseconds} ms.");
				GC.Collect(2, GCCollectionMode.Forced);
				Console.WriteLine($"Total Memory is {GC.GetTotalMemory(true) / 1024 / 1024} Mb.");
			};
			await prepare();
			do
			{
				Console.WriteLine("Enter the word:\n\n");
				var word = Console.ReadLine();
				var start = DateTime.Now;
				var corrections = _services.GetRequiredService<IMorpho>().Get(word);
				var time = (DateTime.Now - start).TotalMilliseconds;
				Console.WriteLine($"\n\n{ (corrections == null ? "---" : string.Join(", ", corrections.Take(200).ToArray()))}\n\n");
				Console.WriteLine($"Testing time is about {time} ms. { (corrections == null ? "---" : $"{corrections.Length.ToString()} items.")}\n\n");
			}
			while (!string.Equals(Console.ReadLine(), "quit", StringComparison.OrdinalIgnoreCase));
		}
	}
}
