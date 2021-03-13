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
}
