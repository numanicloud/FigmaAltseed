using System.Collections.Generic;
using FigmaAltseed.Converter.Abstraction;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Converter.Steps.Symbols;
using FigmaAltseed.Records;
using FigmaSharp.Models;
using Imfact.Annotations;

namespace FigmaAltseed.Infrastructure
{
	[Factory]
	internal partial class StepFactory
	{
		[Resolution(typeof(RestApiStep))]
		public partial IPipelineStep<FigmaCanvas> Canvas(StartupOption option);

		[Resolution(typeof(SymbolStep))]
		public partial IPipelineStep<IVisualSymbols> Symbols(FigmaCanvas canvas);

		[Resolution(typeof(RecordStep))]
		public partial IPipelineStep<FigmaEmptyNode> Record(FigmaCanvas canvas, IVisualSymbols symbols);

		[Resolution(typeof(SvgStep))]
		public partial IPipelineStep<IEnumerable<SvgFileInfo>> Svg(FigmaCanvas canvas, IVisualSymbols symbols);

		[Resolution(typeof(PngStep))]
		public partial IPipelineStep<IEnumerable<PngFileInfo>> Png(IEnumerable<SvgFileInfo> svg);

		[Resolution(typeof(AltTransformStep))]
		public partial IPipelineStep<FigmaEmptyNode> AltTransform(FigmaCanvas canvas, FigmaEmptyNode records);
	}
}
