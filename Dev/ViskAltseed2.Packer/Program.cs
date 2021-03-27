using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ViskAltseed2.Packer
{
	public class Program
	{
		public const string AppName = "ViskAltseed2.Packer";

		public static async Task Main(string[] args)
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
