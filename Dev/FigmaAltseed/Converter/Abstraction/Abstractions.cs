using System.Collections.Generic;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	interface IPipelineStep<out T>
	{
		T Supply();
	}

	internal interface IPipelineFactory
	{
		IPipelineStep<FigmaCanvas> GetCanvasStep(StartupOption option);
		IPipelineStep<ComponentSymbols> GetSymbolStep(FigmaCanvas canvas);
		IPipelineStep<FigmaEmptyNode> GetRecordStep(FigmaCanvas canvas, ComponentSymbols symbols);
		IPipelineStep<IEnumerable<SvgFileInfo>> GetSvgStep(FigmaCanvas canvas, ComponentSymbols symbols);
		IPipelineStep<IEnumerable<PngFileInfo>> GetPngStep(IEnumerable<SvgFileInfo> svg);
		IPipelineStep<FigmaEmptyNode> GetAltTransformStep(FigmaCanvas canvas, FigmaEmptyNode root);
	}

	internal class PipelineFactory : IPipelineFactory
	{
		public IPipelineStep<FigmaCanvas> GetCanvasStep(StartupOption option)
			=> new RestApiStep(option);

		public IPipelineStep<ComponentSymbols> GetSymbolStep(FigmaCanvas canvas)
			=> new SymbolStep(canvas);

		public IPipelineStep<FigmaEmptyNode> GetRecordStep(FigmaCanvas canvas, ComponentSymbols symbols)
			=> new RecordStep(canvas);

		public IPipelineStep<IEnumerable<SvgFileInfo>> GetSvgStep(FigmaCanvas canvas, ComponentSymbols symbols)
			=> new SvgStep(canvas);

		public IPipelineStep<IEnumerable<PngFileInfo>> GetPngStep(IEnumerable<SvgFileInfo> svg)
			=> new PngStep(svg);

		public IPipelineStep<FigmaEmptyNode> GetAltTransformStep(FigmaCanvas canvas, FigmaEmptyNode root)
			=> new AltTransformStep(root, canvas);
	}
}
