using Visklusa.Abstraction.Archiver;
using Visklusa.Abstraction.Notation;
using Visklusa.Abstraction.Variant;
using Visklusa.JsonAltseed.Archiver;
using Visklusa.Notation.Json;

namespace Visklusa.JsonAltseed
{
	public class JsonAltseedVariant : IVisklusaVariant
	{
		private readonly string _visklusaPath;
		private readonly JsonCapabilityRepository _repository;

		public JsonAltseedVariant(string visklusaPath, JsonCapabilityRepository repository)
		{
			_visklusaPath = visklusaPath;
			_repository = repository;
		}

		public IArchiveReader GetPackageReader() => new Altseed2ArchiveReader(_visklusaPath);

		public IArchiveWriter GetPackageWriter() => new Altseed2ArchiveWriter(_visklusaPath);

		public IDeserializer GetDeserializer()
		{
			var result = new JsonLayoutSerializer(_repository);
			result.Options.IgnoreReadOnlyProperties = true;
			return result;
		}

		public ISerializer GetSerializer()
		{
			var result = new JsonLayoutSerializer(_repository);
			result.Options.IgnoreReadOnlyProperties = true;
			return result;
		}

		public string LayoutFileName { get; } = "layout.json";
	}
}
