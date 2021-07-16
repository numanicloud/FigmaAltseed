using Altseed2;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	internal sealed class Altseed2AssetReader : IAssetReader
	{
		public Altseed2AssetReader(string filePath)
		{
			FilePath = filePath;
		}

		public byte[] Read()
		{
			return StaticFile.Create(FilePath).Buffer;
		}

		public string FilePath { get; }
	}
}
