using System;
using System.Collections.Generic;
using FigmaAltseed.Converter.Abstraction;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter
{
	internal class MainConverter
	{
		private readonly PackageSerializer _serializer;
		private readonly IPipelineFactory _factory = new PipelineFactory();

		public MainConverter(PackageSerializer serializer)
		{
			_serializer = serializer;
		}

		public void ConvertToAltseed(StartupOption option)
		{
			var canvas = Canvas(option);
			var symbols = Symbols(canvas);
			var records = Records(canvas, symbols);
			var svg = SvgData(canvas, symbols);
			var png = PngData(svg);
			var altTransform = AltTransform(canvas, records);
			_serializer.Save("output/package.zip", records, png);

			Console.WriteLine("パッケージ作成完了");
		}

		private FigmaEmptyNode AltTransform(FigmaCanvas canvas, FigmaEmptyNode records)
			=> _factory.GetAltTransformStep(canvas, records).Supply();

		private IEnumerable<PngFileInfo> PngData(IEnumerable<SvgFileInfo> svg) 
			=> _factory.GetPngStep(svg).Supply();

		private IEnumerable<SvgFileInfo> SvgData(FigmaCanvas canvas, ComponentSymbols symbols) 
			=> _factory.GetSvgStep(canvas, symbols).Supply();

		private FigmaEmptyNode Records(FigmaCanvas canvas, ComponentSymbols symbols) 
			=> _factory.GetRecordStep(canvas, symbols).Supply();

		private FigmaCanvas Canvas(StartupOption option)
			=> _factory.GetCanvasStep(option).Supply();

		private ComponentSymbols Symbols(FigmaCanvas canvas)
			=> _factory.GetSymbolStep(canvas).Supply();
	}
}
