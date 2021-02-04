using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FigmaAltseed.Records;
using Newtonsoft.Json;
using Svg;

namespace FigmaAltseed.Converter.Steps
{
	internal class PackageSerializer
	{
		public void Save(string path, FigmaAltseedNode nodeTree, IEnumerable<PngFileInfo> assets)
		{
			using var file = File.Create(path);
			using var zip = new ZipArchive(file, ZipArchiveMode.Create);
			var images = ArchiveImageAssets(assets, zip);
			ArchiveNodeTree(nodeTree, zip, images);
		}

		private static PngFileInfo[] ArchiveImageAssets(IEnumerable<PngFileInfo> assets, ZipArchive zip)
		{
			var list = new List<PngFileInfo>();

			foreach (var asset in assets)
			{
				var pngFile = zip.CreateEntry(asset.Path);
				using var pngStream = pngFile.Open();
				
				using var memoryStream = new MemoryStream();
				asset.Bitmap.Save(memoryStream, ImageFormat.Png);

				pngStream.Write(memoryStream.GetBuffer());
				list.Add(asset);
			}

			return list.ToArray();
		}

		private static void ArchiveNodeTree(FigmaAltseedNode nodeTree, ZipArchive zip, PngFileInfo[] images)
		{
			var nodeList = nodeTree.Traverse(x => x.Children)
				.OfType<FigmaSpriteNode>()
				.Where(x => images.All(y => y.Path != x.TextureId));
			foreach (var node in nodeList)
			{
				node.TextureId = "";
			}

			var json = JsonConvert.SerializeObject(nodeTree);
			var nodeFile = zip.CreateEntry("nodes.json");

			using var writer = new StreamWriter(nodeFile.Open());
			writer.WriteLine(json);
		}
	}
}
