using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
				}).RunConsoleAsync();
		}
	}
}
