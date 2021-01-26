using System;
using FigmaAltseed.Records;

namespace FigmaAltseed.Converter
{
	internal class MainConverter
	{
		private readonly FigmaApiAgent _apiAgent;
		private readonly JsonToRecord _recordConverter;
		private readonly JsonToSvg _svgConverter;
		private readonly SvgToPng _pngConverter;
		private readonly PackageSerializer _serializer;

		public MainConverter(FigmaApiAgent apiAgent, JsonToRecord recordConverter,
			JsonToSvg svgConverter, SvgToPng pngConverter, PackageSerializer serializer)
		{
			_apiAgent = apiAgent;
			_recordConverter = recordConverter;
			_svgConverter = svgConverter;
			_pngConverter = pngConverter;
			_serializer = serializer;
		}

		public void ConvertToAltseed(StartupOption option)
		{
			var figmaDocument = _apiAgent.Download(option) ?? throw new Exception("Figma APIからファイルを取得できませんでした。");
			var nodes = _recordConverter.GetRecordTree(figmaDocument);
			var svgData = _svgConverter.ExtractSvgImages(figmaDocument.children[0]);
			var bitmapData = _pngConverter.Covert(svgData);
			_serializer.Save("output/package.zip", nodes, bitmapData);

			Console.WriteLine("パッケージ作成完了");
		}
	}
}
