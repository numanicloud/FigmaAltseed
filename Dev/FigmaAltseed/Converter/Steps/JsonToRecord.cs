using System;
using System.Linq;
using FigmaAltseed.Common;
using FigmaAltseed.Converter.Steps.Symbols;
using FigmaAltseed.Records;
using FigmaSharp;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Steps
{
	internal class JsonToRecord
	{
		private readonly FigmaCanvas _canvas;
		private readonly IVisualSymbols _components;

		public JsonToRecord(FigmaCanvas canvas, IVisualSymbols components)
		{
			_canvas = canvas;
			_components = components;
		}

		public FigmaEmptyNode GetRecordTree()
		{
			var nodes = _canvas.children
				.Select(ConvertRecursively)
				.ToArray();

			return new FigmaEmptyNode() { Children = nodes };
		}

		private FigmaAltseedNode ConvertRecursively(FigmaNode pivot)
		{
			var children = pivot.GetChildren<FigmaNode>()
				.Select(ConvertRecursively)
				.ToArray();

			if (pivot.visible && !pivot.IsAltTransform())
			{
				var component = _components.GetMainSymbol(pivot);
				var imagePath = (component is { } cmp ? cmp : pivot).GetImageAssetPath();

				var bound = pivot.GetAbsoluteBounding() ?? new Rectangle(0, 0, 0, 0);

				return new FigmaSpriteNode(imagePath, bound.Origin.ToDotNet(), bound.Size.ToDotNet())
				{
					Name = pivot.name,
					Children = children,
				};
			}

			return new FigmaEmptyNode() {Children = children};
		}
	}
}
