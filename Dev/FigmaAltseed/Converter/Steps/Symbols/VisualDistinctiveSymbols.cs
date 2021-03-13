using System.Collections.Generic;
using FigmaAltseed.Common;
using FigmaSharp;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Steps.Symbols
{
	internal class VisualDistinctiveSymbols : IVisualSymbols
	{
		private readonly Dictionary<VisualId, FigmaNode> _symbols = new();

		public FigmaNode? GetMainSymbol(FigmaNode instanced)
		{
			var id = instanced switch
			{
				FigmaVector vector => VisualId.Make(vector),
				FigmaFrame frame => VisualId.Make(frame),
				_ => VisualId.Empty,
			};

			if (_symbols.TryGet(id) is {} symbol)
			{
				return symbol;
			}

			_symbols.Add(id, instanced);
			return null;
		}
	}

	internal record VisualId(VisualId.Size SizeData, VisualId.Fill FillData, VisualId.Stroke StrokeData)
	{
		public static readonly VisualId Empty = new(new (0, 0), new (), new ());

		internal record Fill;
		internal record Stroke;

		internal record Size(float Width, float Height);
		internal record SolidFill(Color Color) : Fill;
		internal record SolidStroke(Color Color, float Width) : Stroke;
		internal record Color(double R, double G, double B, double A);

		public static VisualId Make(FigmaVector v)
			=> new(GetSize(v.absoluteBoundingBox),
				v.HasFills ? GetFill(v.fills) : new Fill(),
				v.HasStrokes ? GetStroke(v.strokes, v.strokeWeight) : new Stroke());

		public static VisualId Make(FigmaFrame f)
			=> new(GetSize(f.absoluteBoundingBox),
				f.HasFills ? GetFill(f.fills) : new Fill(),
				f.HasStrokes ? GetStroke(f.strokes, f.strokeWeight) : new Stroke());

		private static Size GetSize(Rectangle rectangle)
		{
			return new Size(rectangle.Width, rectangle.Height);
		}

		private static Fill GetFill(FigmaPaint[] paint)
		{
			if (paint[0].type == "IMAGE")
			{
				return new Fill();
			}

			return new SolidFill(GetColor(paint[0]));
		}

		private static Stroke GetStroke(FigmaPaint[] stroke, int weight)
		{
			return new SolidStroke(GetColor(stroke[0]), weight);
		}

		private static Color GetColor(FigmaPaint f)
		{
			return new(f.color.R, f.color.G, f.color.B, f.color.A);
		}
	}

}
