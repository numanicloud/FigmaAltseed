using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using FigmaAltseed.Records;
using Newtonsoft.Json;

namespace FigmaAltseed.Converter
{
	internal class PackageSerializer
	{
		public void Save(string path, FigmaAltseedNode nodeTree, IEnumerable<PngFileInfo> assets)
		{
			using var file = File.Create(path);
			using var zip = new ZipArchive(file, ZipArchiveMode.Create);
			ArchiveNodeTree(nodeTree, zip);
			ArchiveImageAssets(assets, zip);
		}

		private static void ArchiveImageAssets(IEnumerable<PngFileInfo> assets, ZipArchive zip)
		{
			foreach (var asset in assets)
			{
				var pngFile = zip.CreateEntry(asset.Path);
				using var pngStream = pngFile.Open();
				
				using var memoryStream = new MemoryStream();
				asset.Bitmap.Save(memoryStream, ImageFormat.Png);

				pngStream.Write(memoryStream.GetBuffer());
			}
		}

		private static void ArchiveNodeTree(FigmaAltseedNode nodeTree, ZipArchive zip)
		{
			var json = JsonConvert.SerializeObject(nodeTree);
			var nodeFile = zip.CreateEntry("nodes.json");

			using var writer = new StreamWriter(nodeFile.Open());
			writer.WriteLine(json);
		}
	}
}
