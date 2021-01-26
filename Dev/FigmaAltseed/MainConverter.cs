using System;
using FigmaAltseed.Converter;

namespace FigmaAltseed
{
	internal class MainConverter
	{
		private readonly FigmaLoader _loader;
		private readonly FigmaToRecord _recordConverter;
		private readonly FigmaToSvg _svgConverter;
		private readonly SvgToPng _pngConverter;
		private readonly PackageSerializer _serializer;

		public MainConverter(FigmaLoader loader, FigmaToRecord recordConverter,
			FigmaToSvg svgConverter, SvgToPng pngConverter, PackageSerializer serializer)
		{
			_loader = loader;
			_recordConverter = recordConverter;
			_svgConverter = svgConverter;
			_pngConverter = pngConverter;
			_serializer = serializer;
		}

		public void ConvertToAltseed()
		{
			var figmaDocument = _loader.Download() ?? throw new Exception("Figma APIからファイルを取得できませんでした。");
			var nodes = _recordConverter.GetRecordTree(figmaDocument);
			var svgData = _svgConverter.ExtractSvgImages(figmaDocument.children[0]);
			var bitmapData = _pngConverter.Covert(svgData);
			_serializer.Save("package", nodes, bitmapData);

			Console.WriteLine("パッケージ作成完了");
		}
	}
}
