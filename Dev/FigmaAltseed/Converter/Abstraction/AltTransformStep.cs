using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class AltTransformStep : IPipelineStep<FigmaEmptyNode>
	{
		private readonly FigmaEmptyNode _root;
		private readonly FigmaCanvas _canvas;
		private readonly AltTransformLoader _altTransformLoader = new AltTransformLoader();

		public AltTransformStep(FigmaEmptyNode root, FigmaCanvas canvas)
		{
			_root = root;
			_canvas = canvas;
		}

		public FigmaEmptyNode Supply()
		{
			_altTransformLoader.Load(_root, _canvas);
			return _root;
		}
	}
}
