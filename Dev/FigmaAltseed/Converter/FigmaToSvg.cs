using System.Collections.Generic;
using System.Linq;
using FigmaSharp;
using FigmaSharp.Models;
using Svg;

namespace FigmaAltseed
{
	internal record SvgFileInfo(SvgDocument Document, string FilePath);

	internal class FigmaToSvg
	{
		public IEnumerable<SvgFileInfo> ExtractSvgImages(FigmaCanvas figma)
		{
			// ルール： Fill または Strokeの存在する, Textではないノードのみを描画する
			// Figma上で非表示のものは描画しない
			// Fill が Image のものは描画しない

			return figma.children.SelectMany(ExtractSvgImages);
		}

		private IEnumerable<SvgFileInfo> ExtractSvgImages(FigmaNode pivot)
		{
			var result = pivot.GetChildren<FigmaNode>()
				.SelectMany(ExtractSvgImages);

			if (pivot is FigmaFrame frame && RenderFrame(frame) is {} svg1)
			{
				result = result.Append(new SvgFileInfo(svg1, pivot.GetImageAssetPath()));
			}
			else if (pivot is FigmaVector vector && RenderVector(vector) is {} svg2)
			{
				result = result.Append(new SvgFileInfo(svg2, pivot.GetImageAssetPath()));
			}

			return result;
		}

		private SvgDocument? RenderVector(FigmaVector node)
		{
			if (node is FigmaText || !node.HasFills && !node.HasStrokes)
			{
				return null;
			}

			var bound = node.absoluteBoundingBox;
			var fill = node.fills[0];
			var stroke = node.strokes[0];

			var doc = new SvgDocument()
			{
				Width = bound.Width,
				Height = bound.Height,
				ViewBox = new SvgViewBox(0, 0, bound.Width, bound.Height),
			};

			var rectangle = new SvgRectangle()
			{
				Width = bound.Width,
				Height = bound.Height,
				Fill = new SvgColourServer(fill.color.ToDotNet()),
				Stroke = new SvgColourServer(stroke.color.ToDotNet()),
				StrokeWidth = new SvgUnit(SvgUnitType.Pixel, node.strokeWeight),
			};

			if (node is RectangleVector rect)
			{
				rectangle.CornerRadiusX = rect.cornerRadius;
				rectangle.CornerRadiusY = rect.cornerRadius;
			}

			doc.Children.Add(rectangle);

			return doc;
		}

		private SvgDocument? RenderFrame(FigmaFrame frame)
		{
			if (!frame.HasFills && !frame.HasStrokes)
			{
				return null;
			}

			var bound = frame.absoluteBoundingBox;
			var fill = frame.fills[0];
			var stroke = frame.strokes[0];
			var cornerRadius = frame.cornerRadius;

			var doc = new SvgDocument
			{
				Width = bound.Width,
				Height = bound.Height,
				ViewBox = new SvgViewBox(0, 0, bound.Width, bound.Height),
			};

			doc.Children.Add(new SvgRectangle()
			{
				Width = bound.Width,
				Height = bound.Height,
				Fill = new SvgColourServer(fill.color.ToDotNet()),
				Stroke = new SvgColourServer(stroke.color.ToDotNet()),
				StrokeWidth = new SvgUnit(SvgUnitType.Pixel, frame.strokeWeight),
				CornerRadiusX = cornerRadius,
				CornerRadiusY = cornerRadius,
			});

			return doc;
		}
	}
}
