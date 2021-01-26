using System;
using System.Threading;
using System.Threading.Tasks;
using FigmaAltseed.Converter;
using FigmaAltseed.Records;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FigmaAltseed
{
	internal class Startup : IHostedService
	{
		private readonly IHostApplicationLifetime _appLifetime;
		private readonly MainConverter _mainConverter;
		private readonly StartupOption _option;

		public Startup(IOptions<StartupOption> option, IHostApplicationLifetime appLifetime,
			MainConverter mainConverter)
		{
			_appLifetime = appLifetime;
			_mainConverter = mainConverter;
			_option = option.Value;
		}
		
		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (!_option.GetIsValid())
			{
				Console.WriteLine("Usase:");
				Console.WriteLine($"FigmaAltseed {nameof(StartupOption.FileId)}=\"Figma file ID\" {nameof(StartupOption.Token)}=\"Figma user access token\"");

				_appLifetime.StopApplication();
				return Task.CompletedTask;
			}

			_mainConverter.ConvertToAltseed();
			_appLifetime.StopApplication();

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
