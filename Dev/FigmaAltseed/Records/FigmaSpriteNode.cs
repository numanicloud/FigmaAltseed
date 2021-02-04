using System.Numerics;

namespace FigmaAltseed.Records
{
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
}