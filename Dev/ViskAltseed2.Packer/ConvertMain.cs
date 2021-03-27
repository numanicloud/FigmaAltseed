using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FigmaVisk.Capability;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Visklusa.Abstraction.Archiver;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonAltseed;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;

namespace ViskAltseed2.Packer
{
	internal class ConvertMain : IHostedService
	{
		private readonly StartupOption _options;
		private readonly IHostApplicationLifetime _application;

		public ConvertMain(IOptions<StartupOption> options, IHostApplicationLifetime application)
		{
			_options = options.Value;
			_application = application;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				_options.CheckValid();
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
				_application.StopApplication();
				return Task.CompletedTask;
			}

			Run(_options.PackagePath, _options.OutputPath);
			_application.StopApplication();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private void Run(string packagePath, string outputPath)
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));

			var zipJson = new JsonZipVariant(packagePath, repo);
			zipJson.SetOptionModifier(
				options =>
				{
					var result = new JsonSerializerOptions(options);
					result.IgnoreReadOnlyProperties = true;
					return result;
				});
			var zipAltseed = new JsonAltseedVariant(outputPath, repo);

			using var loader = new VisklusaLoader(zipJson);
			using var saver = new VisklusaSaver(zipAltseed);

			var (layout, assets) = Load(loader);
			Save(saver, layout, assets);
		}

		private static void Save(VisklusaSaver saver, Layout layout, IEnumerable<IAssetReader> assets)
		{
			saver.AddLayout(layout);
			foreach (var asset in assets)
			{
				saver.AddAsset(asset.Read(), asset.FilePath);
			}
		}

		private static (Layout layout, IEnumerable<IAssetReader> assets) Load(VisklusaLoader loader)
		{
			var layout = loader.GetLayout();
			var assetList = JsonSerializer.Deserialize<string[]>(loader.GetAsset("assets.json").Read());
			var assets = assetList is null
				? new IAssetReader[0]
				: assetList.Select(loader.GetAsset);
			return (layout, assets);
		}
	}
}
