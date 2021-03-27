using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FigmaAltseed2
{
	public class Program
	{
		public const string AppName = "FigmaAltseed2";

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

	internal class ConvertMain : IHostedService
	{
		private readonly StartupOption _options;
		private readonly IHostApplicationLifetime _application;

		public ConvertMain(IOptions<StartupOption> options, IHostApplicationLifetime application)
		{
			_options = options.Value;
			_application = application;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				_options.CheckValid();
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
				_application.StopApplication();
				return;
			}

			await Run(_options.FileId, _options.Token, _options.OutputPath);
			_application.StopApplication();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private async Task Run(string fileId, string token, string outputPath)
		{
			var figma = "figma.viskjz.tmp";
			var rendered = "rendered.viskjz.tmp";

			await FigmaVisk.Program.Main(
				new []{ $"FileId={fileId}", $"Token={token}", $"OutputPath={figma}" });

			await ViskVectorRenderer.Program.Main(
				new[] {$"PackagePath={figma}", $"OutputPath={rendered}"});

			await ViskAltseed2.Packer.Program.Main(
				new[] {$"PackagePath={rendered}", $"OutputPath={outputPath}"});

			File.Delete(figma);
			File.Delete(rendered);
		}
	}

	internal class StartupOption
	{
		public string? FileId { get; set; }
		public string? Token { get; set; }
		public string? OutputPath { get; set; }

		[MemberNotNull(nameof(FileId), nameof(Token), nameof(OutputPath))]
		public void CheckValid()
		{
			if (FileId is null || Token is null || OutputPath is null)
			{
				throw new InvalidOperationException(
					$"Usase:\n{Program.AppName} " +
					$"{nameof(FileId)}=\"figma file id\" " +
					$"{nameof(Token)}=\"figma access token\" " +
					$"{nameof(OutputPath)}=\"output package path\"");
			}
		}
	}
}
