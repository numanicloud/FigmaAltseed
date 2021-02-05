using FigmaAltseed.Converter.Steps;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class SymbolStep : IPipelineStep<ComponentSymbols>
	{
		private readonly FigmaCanvas _canvas;

		public SymbolStep(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public ComponentSymbols Supply()
		{
			return new ComponentSymbols(_canvas);
		}
	}
}
