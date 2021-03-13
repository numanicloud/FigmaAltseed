using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Steps.Symbols
{
	internal interface IVisualSymbols
	{
		FigmaNode? GetMainSymbol(FigmaNode instanced);
	}
}
