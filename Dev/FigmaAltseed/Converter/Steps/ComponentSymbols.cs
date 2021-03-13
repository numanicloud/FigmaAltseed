using System.Collections.Generic;
using System.Linq;
using FigmaAltseed.Common;
using FigmaAltseed.Converter.Steps.Symbols;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Steps
{
	internal class ComponentSymbols : IVisualSymbols
	{
		private readonly FigmaCanvas _canvas;
		private readonly Dictionary<string, FigmaNode> _cache = new();

		public ComponentSymbols(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public FigmaNode? GetMainSymbol(FigmaNode instanced)
		{
			return _cache.TryGet(GetSymbolKey(instanced)) is { } cached ? cached
				: FindMainSymbol(instanced) is { } symbol ? symbol
				: null;
		}

		private string GetSymbolKey(FigmaNode node)
		{
			return node.id.Split(";").Last();
		}

		private FigmaNode? FindMainSymbol(FigmaNode instanced)
		{
			if (instanced is FigmaInstance instance)
			{
				return _canvas.FindFirst<FigmaNode>(instance.componentId);
			}

			foreach (var pattern in SplitId(instanced.id))
			{
				// 最初に見つかったInstanceに対してコンポーネントを、そしてシンボルを突き止める
				if (_canvas.FindFirst<FigmaInstance>(pattern.former) is null)
				{
					continue;
				}

				var symbolId = GetSymbolKey(instanced);
				var symbol = _canvas.FindFirst<FigmaNode>(symbolId)
				             ?? _canvas.FindFirst<FigmaNode>("I" + symbolId);

				if (symbol is not null)
				{
					_cache[symbolId] = symbol;
				}

				return symbol;
			}

			return null;
		}

		private IEnumerable<(string former, string latter)> SplitId(string id)
		{
			var split = id.TrimStart('I').Split(";");

			for (int i = 0; i < split.Length; i++)
			{
				var former = string.Join(";", split[..i]);
				var latter = string.Join(";", split[i..]);
				yield return (former, latter);
			}
		}
	}
}
