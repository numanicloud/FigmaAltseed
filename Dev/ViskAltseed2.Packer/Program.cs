using System.Threading.Tasks;
using Altseed2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ViskAltseed2.Packer
{
	class Program
	{
		public const string AppName = "ViskAltseed2.Packer";

		static async Task Main(string[] args)
		{
			await Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(builder =>
				{
					builder.AddCommandLine(args);
				})
				.ConfigureServices((context, collection) =>
				{
					collection.Configure<StartupOption>(context.Configuration);
					collection.AddHostedService<ConvertMain>();
				}).RunConsoleAsync();
		}
	}
}
