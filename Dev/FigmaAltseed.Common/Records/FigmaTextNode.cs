using System.Drawing;
using System.Numerics;

namespace FigmaAltseed.Common.Records
{
	public class FigmaTextNode : FigmaAltseedNode
	{
		public string FontFamily { get; }
		public string Text { get; }
		public Vector2 Position { get; }
		public Color Color { get; }

		public FigmaTextNode(string fontFamily, string text, Vector2 position, Color color)
		{
			FontFamily = fontFamily;
			Text = text;
			Position = position;
			Color = color;
		}
	}
}