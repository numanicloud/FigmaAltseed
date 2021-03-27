using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ViskVectorRenderer
{
	class Program
	{
		public const string AppName = "ViskVectorRenderer";

		static async Task Main(string[] args)
		{
			await Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(
					builder =>
					{
						builder.AddCommandLine(args);
					})
				.ConfigureServices(
					(context, collection) =>
					{
						collection.Configure<StartupOption>(context.Configuration);
						collection.AddHostedService<ConvertMain>();
					})
				.RunConsoleAsync();
		}
	}
}
