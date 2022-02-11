using Altseed2;
using ViskAltseed2;
using Visklusa.JsonAltseed;

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

			var bottom = (SpriteNode)loaded.PathToNode["ClientArea/Background/YellowOverlay"];
			bottom.ZOrder = 1;

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
			return loader.LoadNodes(repo => new JsonAltseedVariant("layout.viskjz", repo));
		}
	}
}
