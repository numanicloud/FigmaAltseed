using Visklusa.Abstraction.Notation;

namespace FigmaVisk.Capability
{
	public record BoundingBox(float X, float Y, float Width, float Height) : ICapability
	{
		public const string Id = "FigmaVisk.BoundingBox";
		public string CapabilityId => Id;
	}

	public record ZOffset(float Z) : ICapability
	{
		public const string Id = "FigmaVisk.ZOffset";
		public string CapabilityId => Id;
	}

	public record Paint(Fill Fill, Stroke Stroke) : ICapability
	{
		public const string Id = "FigmaVisk.Paint";
		public string CapabilityId => Id;
	}

	public record Fill(float Red, float Green, float Blue, float Alpha)
	{
		public static Fill Blank => new Fill(0, 0, 0, 0);
	}

	public record Stroke(float Red, float Green, float Blue, float Alpha, int Weight)
	{
		public static Stroke Blank => new Stroke(0, 0, 0, 0, 0);
	}

	public record RoundedRectangle(float LeftTop, float RightTop, float RightBottom, float LeftBottom) : ICapability
	{
		public const string Id = "FigmaVisk.RoundedRectangle";
		public string CapabilityId => Id;

		public RoundedRectangle() : this(0, 0, 0, 0)
		{
		}

		public RoundedRectangle(float radius)
			: this(radius, radius, radius, radius)
		{
		}
	}

	public record Image(string AssetPath) : ICapability
	{
		public const string Id = "FigmaVisk.Image";
		public string CapabilityId => Id;
	}

	public record Text(string Content, string FontFamily, int FontSize, Fill Fill) : ICapability
	{
		public const string Id = "FigmaVisk.Text";
		public string CapabilityId => Id;
	}

	public record AltPosition(float X, float Y) : ICapability
	{
		public const string Id = "FigmaVisk.AltPosition";
		public string CapabilityId => Id;
	}

	public record FigmaId(string NodeId, string Name) : ICapability
	{
		public const string Id = "FigmaVisk.FigmaId";
		public string CapabilityId => Id;
	}

	public record FamilyShip(string ParentsNodeId) : ICapability
	{
		public const string Id = "FigmaVisk.FamilyShip";
		public string CapabilityId => Id;
	}
}
