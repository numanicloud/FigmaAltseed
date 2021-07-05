using System;
using Altseed2;
using ViskAltseed2;

namespace FigmaAltseed2.Try
{
	class Program
	{
		static void Main(string[] args)
		{
			var result = Engine.Initialize("FigmaAltseed2.Try", 1280, 720);

			var nodes = LoadLayout();
			foreach (var node in nodes)
			{
				Engine.AddNode(node);
			}

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

		private static Node[] LoadLayout()
		{
			var loader = new VisklusaNodeLoader();
			return loader.LoadNodes("figma.viskja");
		}
	}
}
