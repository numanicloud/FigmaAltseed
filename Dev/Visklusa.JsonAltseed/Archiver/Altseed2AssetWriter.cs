using System;
using System.IO;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	class Altseed2AssetWriter : IAssetWriter
	{
		public Altseed2AssetWriter(string filePath)
		{
			FilePath = filePath;
		}

		public void Write(ReadOnlySpan<byte> data)
		{
			using var file = File.Create(FilePath);
			file.Write(data);
		}

		public string FilePath { get; }
	}
}
