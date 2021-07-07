using Altseed2;
using ViskAltseed2;

namespace FigmaAltseed2.Try
{
	class Program
	{
		static void Main(string[] args)
		{
			Engine.Initialize("FigmaAltseed2.Try", 1280, 720);

			var loaded = LoadLayout();
			foreach (var node in loaded.Nodes)
			{
				Engine.AddNode(node);
			}

			Engine.Update();

			var bottom = (SpriteNode)loaded.PathToNode["ClientArea/BottomUI"];
			bottom.Position += new Vector2F(100, 0);

			try
			{
				while (Engine.DoEvents())
				{
					Engine.Update();
				}
			}
			finally
			{
				Engine.Terminate();
			}
		}

		private static LoadResult LoadLayout()
		{
			var loader = new VisklusaNodeLoader();
			return loader.LoadNodes("figma.viskja");
		}
	}
}
