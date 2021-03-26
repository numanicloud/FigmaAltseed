using System.Text.Json;
using Visklusa.Abstraction.Notation;
using Visklusa.Notation.Json;

namespace FigmaVisk.Capability
{
	public class NumericCapabilityConverter<T> : IJsonCapabilityConverter
		where T : ICapability
	{
		public NumericCapabilityConverter(string id)
		{
			Id = id;
		}

		public string Id { get; }

		public ICapability? Read(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			var opt = new JsonSerializerOptions(options);
			opt.IncludeFields = true;
			return JsonSerializer.Deserialize<T>(ref reader, opt);
		}

		public void Write(Utf8JsonWriter writer, ICapability value, JsonSerializerOptions options)
		{
			var opt = new JsonSerializerOptions(options);
			opt.IncludeFields = true;

			var typed = (T) value;
			JsonSerializer.Serialize(writer, typed, opt);
		}
	}
}
