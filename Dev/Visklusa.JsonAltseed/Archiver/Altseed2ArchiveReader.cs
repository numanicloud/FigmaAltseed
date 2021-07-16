using Altseed2;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	class Altseed2ArchiveReader : IArchiveReader
	{
		private readonly string _archivePath;

		public Altseed2ArchiveReader(string archivePath)
		{
			_archivePath = archivePath;
			Engine.File.AddRootPackage(archivePath);
		}

		public void Dispose()
		{
			Engine.File.ClearRootDirectories();
		}

		public IAssetReader GetAsset(string filePath)
		{
			return new Altseed2AssetReader(filePath, _archivePath);
		}
	}
}
