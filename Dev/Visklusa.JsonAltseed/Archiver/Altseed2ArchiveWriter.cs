using System.IO;
using Altseed2;
using Visklusa.Abstraction.Archiver;

namespace Visklusa.JsonAltseed.Archiver
{
	internal class Altseed2ArchiveWriter : IArchiveWriter
	{
		private readonly string _archivePath;
		private readonly string _tmpDirPath;

		public Altseed2ArchiveWriter(string archivePath)
		{
			_archivePath = archivePath;
			_tmpDirPath = archivePath + "_tmp.tmp";

			Directory.CreateDirectory(_tmpDirPath);
			Engine.Initialize(nameof(Altseed2ArchiveWriter), 640, 480);
		}

		public void Dispose()
		{
			Engine.File.Pack(_tmpDirPath, _archivePath);
			Engine.Terminate();
			Directory.Delete(_tmpDirPath, true);
		}

		public IAssetWriter GetAssetWriter(string filePath)
		{
			return new Altseed2AssetWriter(Path.Combine(_tmpDirPath, filePath));
		}
	}
}
