using Altseed2;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	class Altseed2AssetReader : IAssetReader
	{
		private readonly string _archivePath;

		public Altseed2AssetReader(string filePath, string archivePath)
		{
			_archivePath = archivePath;
			FilePath = filePath;
		}

		public byte[] Read()
		{
			Engine.File.AddRootPackage(_archivePath);
			return StaticFile.Create(FilePath).Buffer;
		}

		public string FilePath { get; }
	}
}
