using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json;
using FigmaVisk.Capability;
using Svg.Exceptions;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;
using Image = FigmaVisk.Capability.Image;

namespace ViskVectorRenderer
{
	internal class Serializer
	{
		public void Save(IEnumerable<GroupRenderResult> results, string outputPath)
		{
			var resultArray = results.ToArray();
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));
			repo.Register(new JsonCapabilityBase<FigmaId>(FigmaId.Id));
			repo.Register(new JsonCapabilityBase<AltPosition>(AltPosition.Id));
			repo.Register(new JsonCapabilityBase<FamilyShip>(FamilyShip.Id));

			var variant = new JsonZipVariant(outputPath, repo);
			variant.SetOptionModifier(
				option =>
				{
					var result = new JsonSerializerOptions(option);
					result.IgnoreReadOnlyProperties = true;
					return result;
				});

			using var serializer = new VisklusaSaver(variant);

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

			serializer.AddLayout(
				new Layout(
					new CapabilityAssertion(
						new[]
						{
							BoundingBox.Id, ZOffset.Id, Image.Id, Text.Id,
							FigmaId.Id, AltPosition.Id, FamilyShip.Id
						}),
					resultArray.SelectMany(x => x.Elements).ToArray()));
		}
	}
}
