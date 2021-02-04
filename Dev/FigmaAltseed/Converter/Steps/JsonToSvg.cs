using System.Collections.Generic;
using System.Linq;
using FigmaSharp;
using FigmaSharp.Models;
using Svg;

namespace FigmaAltseed.Converter
{
	internal record SvgFileInfo(SvgDocument Document, string FilePath);

	internal class JsonToSvg
	{
		private readonly SvgRenderer _renderer = new SvgRenderer();
		
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

			if (!pivot.visible)
			{
				return result;
			}

			if (pivot is FigmaFrame frame && RenderFrame2(frame) is {} svg1)
			{
				result = result.Append(new SvgFileInfo(svg1, pivot.GetImageAssetPath()));
			}
			else if (pivot is FigmaVector vector && RenderVector2(vector) is {} svg2)
			{
				result = result.Append(new SvgFileInfo(svg2, pivot.GetImageAssetPath()));
			}

			return result;
		}

		private SvgDocument? RenderVector2(FigmaVector node)
		{
			if (node is FigmaText)
			{
				return null;
			}

			var (doc, rect) = _renderer.CreateRectangleSvg(node.absoluteBoundingBox);
			var updated1 = _renderer.ApplyFill(rect, node.fills);
			var updated2 = _renderer.ApplyStroke(rect, node.strokes, node.strokeWeight);

			if (node is RectangleVector rv)
			{
				_renderer.ApplyCornerRadius(rect, rv.cornerRadius);
			}

			if (updated1.Merge(updated2) == ApplyResult.Changed)
			{
				return doc;
			}

			return null;
		}

		private SvgDocument? RenderFrame2(FigmaFrame node)
		{
			var (doc, rect) = _renderer.CreateRectangleSvg(node.absoluteBoundingBox);
			var updated1 = _renderer.ApplyFill(rect, node.fills);
			var updated2 = _renderer.ApplyStroke(rect, node.strokes, node.strokeWeight);
			_renderer.ApplyCornerRadius(rect, node.cornerRadius);

			if (updated1.Merge(updated2) == ApplyResult.Changed)
			{
				return doc;
			}

			return null;
		}
	}
}
