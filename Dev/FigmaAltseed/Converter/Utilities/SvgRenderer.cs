using System.Linq;
using FigmaAltseed.Common;
using FigmaSharp;
using FigmaSharp.Models;
using Svg;
using static FigmaAltseed.Converter.Utilities.ApplyResult;

namespace FigmaAltseed.Converter.Utilities
{
	internal enum ApplyResult
	{
		Changed, NotChanged
	}

	internal static class ApplyResultHelper
	{
		public static ApplyResult Merge(this ApplyResult self, params ApplyResult[] others)
		{
			if (self == Changed)
			{
				return Changed;
			}

			foreach (var applyResult in others)
			{
				if (applyResult == Changed)
				{
					return Changed;
				}
			}

			return NotChanged;
		}
	}

	internal class SvgRenderer
	{
		public (SvgDocument document, SvgRectangle rectangle) CreateRectangleSvg(Rectangle bound)
		{
			var doc = new SvgDocument()
			{
				X = Pixel(0), Y = Pixel(0),
				Width = Pixel(bound.Width), Height = Pixel(bound.Height),
			};

			var rectangle = new SvgRectangle()
			{
				X = Pixel(0), Y = Pixel(0),
				Width = Pixel(bound.Width), Height = Pixel(bound.Height),
			};

			doc.Children.Add(rectangle);

			return (doc, rectangle);
		}

		public ApplyResult ApplyFill(SvgRectangle rectangle, FigmaPaint[] paints)
		{
			if (!paints.Any())
			{
				return NotChanged;
			}

			var fill = paints[0];

			if (fill.type == "IMAGE")
			{
				return NotChanged;
			}

			rectangle.Fill = new SvgColourServer(fill.color.ToDotNet());

			return Changed;
		}

		public ApplyResult ApplyStroke(SvgRectangle rectangle, FigmaPaint[] color, int width)
		{
			if (!color.Any())
			{
				return NotChanged;
			}

			var stroke = color[0];

			rectangle.Stroke = new SvgColourServer(stroke.color.ToDotNet());
			rectangle.StrokeWidth = Pixel(width);

			// ストロークの太さの分だけ調整
			var wpx = Pixel(width);

			rectangle.X += wpx / 2;
			rectangle.Y += wpx / 2;
			rectangle.OwnerDocument.Width += wpx;
			rectangle.OwnerDocument.Height += wpx;

			return Changed;
		}

		private static SvgUnit Pixel(float width)
		{
			return new SvgUnit(SvgUnitType.Pixel, width);
		}

		public ApplyResult ApplyCornerRadius(SvgRectangle rectangle, float cornerRadius)
		{
			rectangle.CornerRadiusX = cornerRadius;
			rectangle.CornerRadiusY = cornerRadius;
			return NotChanged;
		}
	}
}
