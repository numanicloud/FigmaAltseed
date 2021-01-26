using System.Drawing;
using System.Numerics;

namespace FigmaAltseed.Records
{
	public abstract class FigmaAltseedNode
	{
		public FigmaAltseedNode[] Children { get; set; } = new FigmaAltseedNode[0];
	}

	public class FigmaEmptyNode : FigmaAltseedNode
	{
	}

	public class FigmaSpriteNode : FigmaAltseedNode
	{
		public string TextureId { get; }
		public Vector2 Position { get; }
		public Vector2 SourceArea { get; }

		public FigmaSpriteNode(string textureId, Vector2 position, Vector2 sourceArea)
		{
			TextureId = textureId;
			Position = position;
			SourceArea = sourceArea;
		}
	}

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