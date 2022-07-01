using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FigmaVisk.Capability;
using Svg;
using Visklusa.Abstraction.Notation;

namespace ViskVectorRenderer
{
	// ElementをPaintの内容ごとにグループ化するステップ
	// Elementに対する画像を生成するステップ
	// Elementと画像パスを1対1対応にするステップ
	// シリアライズ

	internal class VectorRenderer
	{
		public IEnumerable<GroupRenderResult> RunGroup(IEnumerable<Element[]> grouped)
		{
			foreach (var elements in grouped)
			{
				var result = Run(elements.Take(1).ToArray()).First();
				yield return result switch
				{
					SuccessRenderResult success =>
						new ImageGroupRenderResult(elements, success.SvgDocument, success.FilePath),
					_ => new SkipGroupRenderResult(elements),
				};
			}
		}

		public IEnumerable<RenderResult> Run(Element[] elements)
		{
			foreach (var element in elements)
			{
				if (element.GetCapability<Paint>() is {} paint)
				{
					yield return RenderRoundedRectangle(element, paint);
				}
				else
				{
					yield return new SkipRenderResult(element);
				}
			}
		}

		private RenderResult RenderRoundedRectangle(Element element, Paint paint)
		{
			if (element.GetCapability<BoundingBox>() is not {} bound
				|| (paint.Fill == Fill.Blank && paint.Stroke == Stroke.Blank)
				|| element.GetCapability<RoundedRectangle>() is not {} rounded)
			{
				return new SkipRenderResult(element);
			}

			var doc = new SvgDocument()
			{
				X = Pixel(0), Y = Pixel(0), Width = Pixel(bound.Width), Height = Pixel(bound.Height)
			};

			var rectangle = new SvgRectangle()
			{
				X = Pixel(0), Y = Pixel(0), Width = Pixel(bound.Width), Height = Pixel(bound.Height),
				Fill = new SvgColourServer(GetColor(paint.Fill)),
				Stroke = new SvgColourServer(GetColor(paint.Stroke)),
				StrokeWidth = Pixel(paint.Stroke.Weight),
			};
			
			rectangle.CornerRadiusX = Pixel(rounded.LeftBottom);
			rectangle.CornerRadiusY = Pixel(rounded.LeftBottom);

			if (paint.Stroke != Stroke.Blank)
			{
				var wpx = Pixel(paint.Stroke.Weight);
				rectangle.X += wpx / 2;
				rectangle.Y += wpx / 2;
				doc.Width += wpx;
				doc.Height += wpx;
			}

			doc.Children.Add(rectangle);

			return new SuccessRenderResult(element, doc, $"rendered_{element.Id}.png");
		}

		private SvgUnit Pixel(float value)
		{
			return new SvgUnit(SvgUnitType.Pixel, value);
		}

		private Color GetColor(Fill f)
		{
			var (r, g, b, a) = f;
			var (a1, r1, g1, b1) = (a, r, g, b).Map(x => (int)(x * 255));
			return Color.FromArgb(a1, r1, g1, b1);
		}

		private Color GetColor(Stroke s)
		{
			var (r, g, b, a, w) = s;
			var (a1, r1, g1, b1) = (a, r, g, b).Map(x => (int) (x * 255));
			return Color.FromArgb(a1, r1, g1, b1);
		}
	}
}
