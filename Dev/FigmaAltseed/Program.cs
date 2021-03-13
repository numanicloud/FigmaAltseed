using System.Threading.Tasks;
using FigmaAltseed.Converter;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FigmaAltseed
{
	internal class Program
	{
		internal static async Task Main(string[] args)
		{
			await Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(builder =>
				{
					builder.AddCommandLine(args);
				})
				.ConfigureServices((context, collection) =>
				{
					collection.Configure<StartupOption>(context.Configuration);
					collection.AddHostedService<Startup>();
					collection.AddSingleton<PackageSerializer>();
					collection.AddSingleton<MainConverter>();
				}).RunConsoleAsync();
		}
	}
}
