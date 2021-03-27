using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FigmaVisk.Capability;
using Svg.Exceptions;
using Visklusa.Abstraction.Notation;
using Visklusa.JsonZip;
using Visklusa.Notation.Json;
using Image = FigmaVisk.Capability.Image;

namespace ViskVectorRenderer
{
	internal class Serializer
	{
		public void Save(IEnumerable<RenderResult> results, string outputPath)
		{
			var resultArray = results.ToArray();
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Paint>(Paint.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));

			using var serializer = ViskJzFactory.GetSaver(outputPath, repo);

			foreach (var renderResult in resultArray)
			{
				if (renderResult is SuccessRenderResult success)
				{
					Bitmap bitmap;
					try
					{
						bitmap = success.SvgDocument.Draw();
					}
					catch (SvgMemoryException memoryException)
					{
						Console.WriteLine($"{memoryException}; ElementId={renderResult.Element.Id}");
						continue;
					}

					using var memoryStream = new MemoryStream();
					bitmap.Save(memoryStream, ImageFormat.Png);

					var path = $"rendered_{renderResult.Element.Id}";
					serializer.AddAsset(memoryStream.GetBuffer(), path);
				}
			}

			serializer.AddLayout(
				new Layout(
					new CapabilityAssertion(
						new[]
						{
							BoundingBox.Id, ZOffset.Id, Paint.Id, Image.Id
						}),
					resultArray.Select(x => x.Element).ToArray()));
		}
	}
}
