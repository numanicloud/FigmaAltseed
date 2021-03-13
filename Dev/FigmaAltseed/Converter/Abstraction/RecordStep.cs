using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Converter.Steps.Symbols;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class RecordStep : IPipelineStep<FigmaEmptyNode>
	{
		private readonly JsonToRecord _jsonToRecord;

		public RecordStep(FigmaCanvas canvas, IVisualSymbols components)
		{
			_jsonToRecord = new JsonToRecord(canvas, components);
		}

		public FigmaEmptyNode Supply()
		{
			return _jsonToRecord.GetRecordTree();
		}
	}
}
