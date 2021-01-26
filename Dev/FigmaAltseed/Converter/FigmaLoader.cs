using FigmaSharp;
using FigmaSharp.Models;
using AppContext = FigmaSharp.AppContext;

namespace FigmaAltseed
{
	internal class FigmaLoader
	{
		private readonly StartupOption _option;

		public FigmaLoader(StartupOption option)
		{
			_option = option;
		}

		public FigmaDocument? Download()
		{
			var query = new FigmaFileQuery(_option.FileId, _option.Token);
			var response = AppContext.Api.GetFile(query);
			return response?.document;
		}
	}
}
