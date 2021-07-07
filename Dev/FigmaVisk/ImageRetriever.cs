using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FigmaVisk
{
	internal class ImageRetriever
	{
		private readonly StartupOption _option;
		private Dictionary<string, string>? _imageMap;

		public ImageRetriever(StartupOption option)
		{
			_option = option;
		}
		
		[MemberNotNull(nameof(_imageMap))]
		public async Task LoadAsync()
		{
			using var http = new HttpClient();
			http.DefaultRequestHeaders.Add("X-FIGMA-TOKEN", _option.Token);

			var requestUri = $"https://api.figma.com/v1/files/{_option.FileId}/images";
			var response = await http.GetStringAsync(requestUri);

			var jsonElements = JsonConvert.DeserializeXNode(response).Root
					?.Element("meta")
					?.Element("images")
					?.Elements()
				?? throw new Exception();

			_imageMap = jsonElements.ToDictionary(x => x.Name.LocalName, x => x.Value);
		}

		public async ValueTask<byte[]> DownloadAsync(string imageRef)
		{
			if (_imageMap is null)
			{
				await LoadAsync();
			}

			using var http = new HttpClient();
			var url = _imageMap[imageRef];
			return await http.GetByteArrayAsync(url);
		}
	}
}
