using System;
using FigmaAltseed.Converter.Steps;
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
		private readonly AltTransformLoader _altTransformLoader;

		public MainConverter(FigmaApiAgent apiAgent, JsonToRecord recordConverter,
			JsonToSvg svgConverter, SvgToPng pngConverter, PackageSerializer serializer,
			AltTransformLoader altTransformLoader)
		{
			_apiAgent = apiAgent;
			_recordConverter = recordConverter;
			_svgConverter = svgConverter;
			_pngConverter = pngConverter;
			_serializer = serializer;
			_altTransformLoader = altTransformLoader;
		}

		public void ConvertToAltseed(StartupOption option)
		{
			var figmaDocument = _apiAgent.Download(option) ?? throw new Exception("Figma APIからファイルを取得できませんでした。");
			var canvas = figmaDocument.children[0];

			var svgData = _svgConverter.ExtractSvgImages(canvas);
			var bitmapData = _pngConverter.Covert(svgData);

			var nodes = _recordConverter.GetRecordTree(canvas);
			_altTransformLoader.Load(nodes, canvas);

			_serializer.Save("output/package.zip", nodes, bitmapData);

			Console.WriteLine("パッケージ作成完了");
		}
	}
}
