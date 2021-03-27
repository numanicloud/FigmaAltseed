using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FigmaVisk.Capability;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Visklusa.Abstraction.Notation;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;

namespace ViskVectorRenderer
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

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				_options.CheckOption();
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
				_application.StopApplication();
				return;
			}

			await RunAsync(_options.OutputPath);
			_application.StopApplication();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private async Task RunAsync(string outputPath)
		{
			var layout = GetLayout();
			var svg = new VectorRenderer();
			var refMod = new ImageReferenceModifier();
			var serializer = new Serializer();

			var grouped = Group(layout.Elements);
			var rendered = svg.RunGroup(grouped);
			var modified = refMod.Apply(rendered);
			serializer.Save(modified, outputPath);
		}

		private IEnumerable<Element[]> Group(Element[] elements)
		{
			return elements.GroupBy(x => x.GetCapability<Paint>())
				.SelectMany(x => x.GroupBy(y => y.GetCapability<RoundedRectangle>()))
				.SelectMany(
					x => x.GroupBy(
						y => y.GetCapability<BoundingBox>() is { } bound
							? bound.Width * 197 + bound.Height * 331
							: -1))
				.Select(x => x.ToArray())
				.ToArray();
		}

		private Layout GetLayout()
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Paint>(Paint.Id));
			repo.Register(new JsonCapabilityBase<RoundedRectangle>(RoundedRectangle.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));

			using var source = ViskJzFactory.GetLoader(_options.PackagePath, repo);

			return source.GetLayout();
		}
	}
}
