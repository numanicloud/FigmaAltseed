using System.Collections.Generic;
using System.Linq;
using FigmaAltseed.Common;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Steps
{
	internal class ComponentLoader
	{
		public void Load(FigmaCanvas canvas)
		{
			var symbols = canvas.TraverseFigma()
				.Where(x => x.type == "COMPONENT")
				.SelectMany(Helper.TraverseFigma)
				.ToDictionary(x => x.id, x => x);
		}
	}

	internal class ComponentSymbols
	{
		private readonly FigmaCanvas _canvas;
		private readonly Dictionary<string, FigmaNode> _cache = new();

		public ComponentSymbols(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public FigmaNode? GetMainSymbol(FigmaNode instanced)
		{
			return !instanced.id.StartsWith("I") ? instanced
				: _cache.TryGet(instanced.id) is { } cached ? cached
				: FindMainSymbol(instanced) is { } symbol ? symbol
				: null;
		}

		private FigmaNode? FindMainSymbol(FigmaNode instanced)
		{
			foreach (var pattern in SplitId(instanced.id))
			{
				// 最初に見つかったInstanceに対してコンポーネントを、そしてシンボルを突き止める
				if (_canvas.FindFirst<FigmaInstance>(pattern.former) is not { } instance)
				{
					continue;
				}

				var symbolId = instance.componentId + ";" + pattern.latter;
				var symbol = _canvas.FindFirst<FigmaNode>(symbolId)
				             ?? _canvas.FindFirst<FigmaNode>("I" + symbolId);

				if (symbol is not null)
				{
					_cache[instanced.id] = symbol;
				}

				return symbol;
			}

			return null;
		}

		private IEnumerable<(string former, string latter)> SplitId(string id)
		{
			var split = id.Split(";");

			for (int i = 0; i < split.Length; i++)
			{
				var former = string.Join(";", split[..i]);
				var latter = string.Join(";", split[i..]);
				yield return (former, latter);
			}
		}
	}
}
