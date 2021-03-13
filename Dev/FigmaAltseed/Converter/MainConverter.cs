using System;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Infrastructure;
using FigmaAltseed.Records;

namespace FigmaAltseed.Converter
{
	internal class MainConverter
	{
		private readonly PackageSerializer _serializer;

		public MainConverter(PackageSerializer serializer)
		{
			_serializer = serializer;
		}

		public void ConvertToAltseed(StartupOption option)
		{
			var f = new StepFactory();

			var canvas = f.Canvas(option).Supply();
			var symbols = f.Symbols(canvas).Supply();
			var records = f.Record(canvas, symbols).Supply();
			var svg = f.Svg(canvas, symbols).Supply();
			var png = f.Png(svg).Supply();
			var altTransform = f.AltTransform(canvas, records).Supply();

			_serializer.Save("output/package.zip", altTransform, png);

			Console.WriteLine("パッケージ作成完了");
		}
	}
}
