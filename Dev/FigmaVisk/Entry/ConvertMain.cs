using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FigmaVisk.Capability;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;

namespace FigmaVisk
{
	internal class ConvertMain : IHostedService
	{
		private readonly StartupOption _option;
		private readonly IHostApplicationLifetime _appLifetime;

		public ConvertMain(IOptions<StartupOption> option, IHostApplicationLifetime appLifetime)
		{
			_option = option.Value;
			_appLifetime = appLifetime;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				_option.CheckOption();
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
				_appLifetime.StopApplication();
				return;
			}

			await RunAsync();
			_appLifetime.StopApplication();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private async Task RunAsync()
		{
			var api = new FigmaApiAgent();
			var analyzer = new DocumentAnalyzer();

			var document = api.Download(_option);
			if (document is null)
			{
				throw new Exception();
			}

			var elements = analyzer.Analyze(document);

			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Paint>(Paint.Id));
			repo.Register(new JsonCapabilityBase<RoundedRectangle>(RoundedRectangle.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));

			var variant = new JsonZipVariant(_option.OutputPath, repo);
			variant.SetOptionModifier(
				option =>
				{
					var result = new JsonSerializerOptions(option);
					result.IgnoreReadOnlyProperties = true;
					return result;
				});

			using var visk = new VisklusaSaver(variant);

			visk.AddLayout(new Layout(
				new CapabilityAssertion(new []
				{
					BoundingBox.Id, ZOffset.Id, Paint.Id, RoundedRectangle.Id, Text.Id
				}), elements));
		}
	}
}
