using System.Numerics;

namespace FigmaAltseed.Common.Records
{
	public class AltTransform
	{
		public AltTransform(Vector2 position)
		{
			Position = position;
		}

		public Vector2 Position { get; }
	}
}