using FigmaAltseed.Records;
using FigmaSharp;
using FigmaSharp.Models;
using AppContext = FigmaSharp.AppContext;

namespace FigmaAltseed.Converter
{
	internal class FigmaApiAgent
	{
		public FigmaDocument? Download(StartupOption option)
		{
			var query = new FigmaFileQuery(option.FileId, option.Token);
			var response = AppContext.Api.GetFile(query);
			return response?.document;
		}
	}
}
