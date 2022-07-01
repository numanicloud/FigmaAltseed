using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json;
using FigmaVisk.Capability;
using Svg.Exceptions;
using Visklusa.Abstraction.Archiver;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;
using Image = FigmaVisk.Capability.Image;

namespace ViskVectorRenderer
{
	internal class Serializer
	{
		private readonly StartupOption _option;

		public Serializer(StartupOption option)
		{
			_option = option;
		}

		public void Save(IEnumerable<GroupRenderResult> results, string outputPath)
		{
			var resultArray = results.ToArray();
			var repo = GetRepository();
			var variant = new JsonZipVariant(outputPath, repo);
			variant.SetOptionModifier(
				option =>
				{
					var result = new JsonSerializerOptions(option);
					result.IgnoreReadOnlyProperties = true;
					return result;
				});
			var loadVariant = new JsonZipVariant(_option.PackagePath, repo);

			using (var serializer = new VisklusaSaver(variant))
			{
				using (var loader = new VisklusaLoader(loadVariant))
				{
					var srcAssets = loader.GetAllAsset();
					WriteSourceAssets(serializer, srcAssets);
				}
				WriteAssets(resultArray, serializer);
				WriteLayout(serializer, resultArray);
			}
		}

		private static JsonCapabilityRepository GetRepository()
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));
			repo.Register(new JsonCapabilityBase<FigmaId>(FigmaId.Id));
			repo.Register(new JsonCapabilityBase<AltPosition>(AltPosition.Id));
			repo.Register(new JsonCapabilityBase<FamilyShip>(FamilyShip.Id));
			repo.Register(new JsonCapabilityBase<VerticalScroll>(VerticalScroll.Id));
			repo.Register(new JsonCapabilityBase<VerticalList>(VerticalList.Id));
			return repo;
		}

		private static void WriteSourceAssets(VisklusaSaver saver, IEnumerable<IAssetReader> readers)
		{
			foreach (var assetReader in readers)
			{
				var bytes = assetReader.Read();
				saver.AddAsset(bytes, assetReader.FilePath);
			}
		}

		private static void WriteLayout(VisklusaSaver serializer, GroupRenderResult[] resultArray)
		{
			serializer.AddLayout(new Layout(new CapabilityAssertion(new[]
				{
					BoundingBox.Id, ZOffset.Id, Image.Id, Text.Id,
					FigmaId.Id, AltPosition.Id, FamilyShip.Id
				}),
				resultArray.SelectMany(x => x.Elements).ToArray()));
		}

		private static void WriteAssets(GroupRenderResult[] resultArray, VisklusaSaver serializer)
		{
			foreach (var renderResult in resultArray)
			{
				if (renderResult is ImageGroupRenderResult image)
				{
					Bitmap bitmap;
					try
					{
						bitmap = image.SvgDocument.Draw();
					}
					catch (SvgMemoryException memoryException)
					{
						Console.WriteLine($"{memoryException}; Name={image.FilePath}");
						continue;
					}

					using var memoryStream = new MemoryStream();
					bitmap.Save(memoryStream, ImageFormat.Png);

					serializer.AddAsset(memoryStream.GetBuffer(), image.FilePath);
				}
			}
		}
	}
}
