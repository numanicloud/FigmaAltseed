using FigmaVisk.Capability;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;

namespace FigmaVisk.Entry
{
	internal class Converter
	{
		private readonly FigmaApiAgent _agent;
		private readonly DocumentAnalyzer _documentAnalyzer;
		private readonly AltTransformAnalyzer _altTransformAnalyzer;
		private readonly ImageInstaller _imageInstaller;

		public Converter(FigmaApiAgent agent, DocumentAnalyzer documentAnalyzer,
			AltTransformAnalyzer altTransformAnalyzer, ImageInstaller imageInstaller)
		{
			_agent = agent;
			_documentAnalyzer = documentAnalyzer;
			_altTransformAnalyzer = altTransformAnalyzer;
			_imageInstaller = imageInstaller;
		}

		public async Task RunAsync(StartupOption option)
		{
			var elements = ScanElements(option);
			var repo = GetCapabilityRepo();
			var variant = GetVariant(option, repo);
			await SaveAsync(variant, elements);
		}

		private NodeExport[] ScanElements(StartupOption option)
		{
			var document = _agent.Download(option);
			if (document is null)
			{
				throw new Exception();
			}

			var elements = _documentAnalyzer.Analyze(document);
			elements = _altTransformAnalyzer.Convert(elements);
			var installations = _imageInstaller.Convert(elements);

			return installations.Select(x => new NodeExport(x)).ToArray();
		}

		private static JsonCapabilityRepository GetCapabilityRepo()
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Paint>(Paint.Id));
			repo.Register(new JsonCapabilityBase<RoundedRectangle>(RoundedRectangle.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));
			repo.Register(new JsonCapabilityBase<FigmaId>(FigmaId.Id));
			repo.Register(new JsonCapabilityBase<AltPosition>(AltPosition.Id));
			repo.Register(new JsonCapabilityBase<FamilyShip>(FamilyShip.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			return repo;
		}

		private static JsonZipVariant GetVariant(StartupOption option, JsonCapabilityRepository repo)
		{
			var variant = new JsonZipVariant(option.OutputPath, repo);
			variant.SetOptionModifier(sOption =>
			{
				var result = new JsonSerializerOptions(sOption);
				result.IgnoreReadOnlyProperties = true;
				return result;
			});
			return variant;
		}

		private static async Task SaveAsync(JsonZipVariant variant, NodeExport[] elements)
		{
			using var visk = new VisklusaSaver(variant);

			visk.AddLayout(new Layout(new CapabilityAssertion(new[]
			{
				BoundingBox.Id, ZOffset.Id, Paint.Id, RoundedRectangle.Id, Text.Id, FigmaId.Id,
				AltPosition.Id, FamilyShip.Id, Image.Id
			}), elements.Select(x => x.Element).ToArray()));

			foreach (var item in elements)
			{
				await item.Installation.OnSaveAsync(visk);
			}
		}

		private record NodeExport(ImageInstaller.IImageInstallation Installation)
		{
			public Element Element => Installation.Element;
		}
	}
}
