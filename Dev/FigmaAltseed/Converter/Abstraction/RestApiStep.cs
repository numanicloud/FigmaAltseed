using System;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	internal class RestApiStep : IPipelineStep<FigmaCanvas>
	{
		private readonly StartupOption _option;
		private readonly FigmaApiAgent _figmaApiAgent = new FigmaApiAgent();

		public RestApiStep(StartupOption option)
		{
			_option = option;
		}

		public FigmaCanvas Supply()
		{
			return _figmaApiAgent.Download(_option)?.children[0]
				?? throw new Exception();
		}
	}
}
