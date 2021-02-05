using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class RecordStep : IPipelineStep<FigmaEmptyNode>
	{
		private readonly FigmaCanvas _canvas;
		private readonly JsonToRecord.Factory _factory = new JsonToRecord.Factory();

		public RecordStep(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public FigmaEmptyNode Supply()
		{
			return _factory.Create(_canvas).GetRecordTree();
		}
	}
}
