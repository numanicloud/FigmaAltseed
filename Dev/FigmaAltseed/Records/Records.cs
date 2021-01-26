using Altseed2;

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
		public Vector2F Position { get; }
		public Vector2F SourceArea { get; }

		public FigmaSpriteNode(string textureId, Vector2F position, Vector2F sourceArea)
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
		public Vector2F Position { get; }
		public Color Color { get; }

		public FigmaTextNode(string fontFamily, string text, Vector2F position, Color color)
		{
			FontFamily = fontFamily;
			Text = text;
			Position = position;
			Color = color;
		}
	}
}