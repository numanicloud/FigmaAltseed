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

		public RoundedRectangle(float radius)
			: this(radius, radius, radius, radius)
		{
			
		}
	}
}
