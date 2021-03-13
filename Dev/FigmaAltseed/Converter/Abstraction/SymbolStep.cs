using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Converter.Steps.Symbols;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class SymbolStep : IPipelineStep<IVisualSymbols>
	{
		private readonly FigmaCanvas _canvas;

		public SymbolStep(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public IVisualSymbols Supply()
		{
			return new VisualDistinctiveSymbols();
		}
	}
}
