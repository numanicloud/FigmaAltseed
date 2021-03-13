using System.Collections.Generic;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Converter.Steps.Symbols;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class SvgStep : IPipelineStep<IEnumerable<SvgFileInfo>>
	{
		private readonly FigmaCanvas _canvas;
		private readonly JsonToSvg _jsonToSvg;

		public SvgStep(FigmaCanvas canvas, IVisualSymbols components)
		{
			_canvas = canvas;
			_jsonToSvg = new(components);
		}

		public IEnumerable<SvgFileInfo> Supply()
		{
			return _jsonToSvg.ExtractSvgImages(_canvas);
		}
	}
}
