using FigmaSharp;
using FigmaSharp.Models;

namespace FigmaVisk
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
