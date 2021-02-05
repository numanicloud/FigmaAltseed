using System.Collections.Generic;
using FigmaAltseed.Converter.Steps;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class PngStep : IPipelineStep<IEnumerable<PngFileInfo>>
	{
		private readonly IEnumerable<SvgFileInfo> _svgData;
		private readonly SvgToPng _svgToPng = new SvgToPng();

		public PngStep(IEnumerable<SvgFileInfo> svgData)
		{
			_svgData = svgData;
		}

		public IEnumerable<PngFileInfo> Supply()
		{
			return _svgToPng.Covert(_svgData);
		}
	}
}
