using Altseed2;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	class Altseed2ArchiveReader : IArchiveReader
	{
		public Altseed2ArchiveReader(string archivePath)
		{
			Engine.Initialize(nameof(Altseed2ArchiveReader), 640, 480);
			Engine.File.AddRootPackage(archivePath);
		}

		public void Dispose()
		{
			Engine.File.ClearRootDirectories();
			Engine.Terminate();
		}

		public IAssetReader GetAsset(string filePath)
		{
			return new Altseed2AssetReader(filePath);
		}
	}
}
