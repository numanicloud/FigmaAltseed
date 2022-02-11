using System.IO;
using Altseed2;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	internal sealed class Altseed2ArchiveReader : IArchiveReader
	{
		public Altseed2ArchiveReader(string archivePath)
		{
			if (!System.IO.File.Exists(archivePath))
			{
				throw new FileNotFoundException("アーカイブファイルが見つかりませんでした。", archivePath);
			}
			
			Engine.File.AddRootPackage(archivePath);
		}

		public void Dispose()
		{
		}

		public IAssetReader GetAsset(string filePath)
		{
			return new Altseed2AssetReader(filePath);
		}
	}
}
