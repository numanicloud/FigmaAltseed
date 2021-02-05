using System.Collections.Generic;
using FigmaAltseed.Converter.Steps;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class SvgStep : IPipelineStep<IEnumerable<SvgFileInfo>>
	{
		private readonly FigmaCanvas _canvas;
		private readonly JsonToSvg _jsonToSvg = new JsonToSvg();

		public SvgStep(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public IEnumerable<SvgFileInfo> Supply()
		{
			return _jsonToSvg.ExtractSvgImages(_canvas);
		}
	}
}
