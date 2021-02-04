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

			return figma.children.Traverse(x => x.GetChildren<FigmaNode>())
				.Where(x => x.visible)
				.Select(ExtractSvgImage)
				.FilterNull();
		}

		private SvgFileInfo? ExtractSvgImage(FigmaNode node)
		{
			var doc = node switch
			{
				FigmaFrame f when RenderFrame(f) is { } svg => svg,
				FigmaVector v when RenderVector(v) is { } svg => svg,
				_ => null
			};

			return doc is { } x ? new SvgFileInfo(x, node.GetImageAssetPath()) : null;
		}

		private SvgDocument? RenderVector(FigmaVector node)
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

			return updated1.Merge(updated2) == ApplyResult.Changed ? doc : null;
		}

		private SvgDocument? RenderFrame(FigmaFrame node)
		{
			var (doc, rect) = _renderer.CreateRectangleSvg(node.absoluteBoundingBox);
			var updated1 = _renderer.ApplyFill(rect, node.fills);
			var updated2 = _renderer.ApplyStroke(rect, node.strokes, node.strokeWeight);
			_renderer.ApplyCornerRadius(rect, node.cornerRadius);

			return updated1.Merge(updated2) == ApplyResult.Changed ? doc : null;
		}
	}
}
