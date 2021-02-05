using System;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Sandbox
{
	/* 特殊な状況下において、他の手法との優劣はどうか？
	メソッドが１つしか無い場合。あるいは、たくさんある場合。
	その文脈に依存するオブジェクトが１つだけの場合。あるいは、たくさんある場合。
	文脈が２段階以上ある場合。あるいは、１段階だけの場合。
	ひし形の文脈を持つ場合。あるいは、持たない場合。
	 */

	internal class RecordPipe
	{
		private readonly ComponentSymbols _symbols;
		private readonly FigmaCanvas _canvas;

		[PipelineInput]
		public RecordPipe(ComponentSymbols symbols, FigmaCanvas canvas)
		{
			_symbols = symbols;
			_canvas = canvas;
		}

		[PipelineOutput("Record")]
		public FigmaAltseedNode Make()
		{
			throw new NotImplementedException();
		}
	}

	internal class SvgPipe
	{
		private readonly ComponentSymbols _symbols;
		private readonly FigmaCanvas _canvas;

		[PipelineInput]
		public SvgPipe(ComponentSymbols symbols, FigmaCanvas canvas)
		{
			_symbols = symbols;
			_canvas = canvas;
		}

		[PipelineOutput("Svg")]
		public SvgFileInfo Make()
		{
			throw new NotImplementedException();
		}
	}

	internal class PipelineResolver
	{
		public FigmaAltseedNode ResolveNode(ComponentSymbols symbols, FigmaCanvas canvas)
		{
			var recordPipe = new RecordPipe(symbols, canvas);
			return recordPipe.Make();
		}
	}

	internal static class Pipeline2
	{
		public static FigmaCanvas Canvas()
		{
			return new FigmaCanvas();
		}

		public static (FigmaCanvas, ComponentSymbols) Symbols(this FigmaCanvas supply)
		{
			return supply.TupleWith(new ComponentSymbols(supply));
		}

		public static (FigmaCanvas, ComponentSymbols, FigmaAltseedNode) Symbols(
			this (FigmaCanvas, ComponentSymbols) supply)
		{
			var pipe = new RecordPipe(supply.Item2, supply.Item1);
			return supply.TupleWith(pipe.Make());
		}

		public static (FigmaCanvas, ComponentSymbols, FigmaAltseedNode, SvgFileInfo) Symbols(
			this (FigmaCanvas, ComponentSymbols, FigmaAltseedNode) supply)
		{
			var pipe = new SvgPipe(supply.Item2, supply.Item1);
			return supply.TupleWith(pipe.Make());
		}

		private static (T1, T2) TupleWith<T1, T2>(this T1 current, T2 tail)
		{
			return (current, tail);
		}

		private static (T1, T2, T3) TupleWith<T1, T2, T3>(this (T1, T2) current, T3 tail)
		{
			return (current.Item1, current.Item2, tail);
		}

		private static (T1, T2, T3, T4) TupleWith<T1, T2, T3, T4>(this (T1, T2, T3) current, T4 tail)
		{
			return (current.Item1, current.Item2, current.Item3, tail);
		}
	}

	internal static class Pipeline
	{
		public static Supply1Canvas Canvas()
		{
			return new Supply1Canvas(new FigmaCanvas());
		}

		public static Supply2Symbol Symbols(this Supply1Canvas supply)
		{
			return new Supply2Symbol(new ComponentSymbols(supply.FigmaCanvas), supply.FigmaCanvas);
		}

		public static Supply3Record Record(this Supply2Symbol supply)
		{
			var pipe = new RecordPipe(supply.ComponentSymbols, supply.FigmaCanvas);
			return new Supply3Record(supply.ComponentSymbols, supply.FigmaCanvas, pipe.Make());
		}

		public static Supply4Svg Svg(this Supply3Record supply)
		{
			var pipe = new SvgPipe(supply.ComponentSymbols, supply.FigmaCanvas);
			return new Supply4Svg(supply.ComponentSymbols,
				supply.FigmaCanvas,
				pipe.Make(),
				supply.FigmaAltseedNode);
		}

		public static Supply5Png Png(this Supply4Svg supply)
		{
			return new Supply5Png(
				supply.ComponentSymbols,
				supply.FigmaCanvas,
				supply.SvgFileInfo,
				supply.FigmaAltseedNode,
				default);
		}

		public static Supply3Svg Svg(this Supply2Symbol supply)
		{
			var pipe = new SvgPipe(supply.ComponentSymbols, supply.FigmaCanvas);
			return new Supply3Svg(supply.ComponentSymbols, supply.FigmaCanvas, pipe.Make());
		}

		public static Supply4Svg Record(this Supply3Svg supply)
		{
			var pipe = new RecordPipe(supply.ComponentSymbols, supply.FigmaCanvas);
			return new Supply4Svg(supply.ComponentSymbols,
				supply.FigmaCanvas,
				supply.SvgFileInfo,
				pipe.Make());
		}

		public static Supply4Png Png(this Supply3Svg supply)
		{
			return new Supply4Png(supply.ComponentSymbols,
				supply.FigmaCanvas,
				supply.SvgFileInfo,
				default);
		}

		public static Supply5Png Record(this Supply4Png supply)
		{
			return new Supply5Png(
				supply.ComponentSymbols,
				supply.FigmaCanvas,
				supply.SvgFileInfo,
				default,
				supply.PngFileInfo);
		}

		public record Supply1Canvas(FigmaCanvas FigmaCanvas);

		public record Supply2Symbol(ComponentSymbols ComponentSymbols, FigmaCanvas FigmaCanvas);

		public record Supply3Record(
				ComponentSymbols ComponentSymbols,
				FigmaCanvas FigmaCanvas,
				FigmaAltseedNode FigmaAltseedNode);

		public record Supply3Svg(
				ComponentSymbols ComponentSymbols,
				FigmaCanvas FigmaCanvas,
				SvgFileInfo SvgFileInfo);

		public record Supply4Svg(
			ComponentSymbols ComponentSymbols,
			FigmaCanvas FigmaCanvas,
			SvgFileInfo SvgFileInfo,
			FigmaAltseedNode FigmaAltseedNode);

		public record Supply4Png(
			ComponentSymbols ComponentSymbols,
			FigmaCanvas FigmaCanvas,
			SvgFileInfo SvgFileInfo,
			PngFileInfo PngFileInfo);

		public record Supply5Png(
			ComponentSymbols ComponentSymbols,
			FigmaCanvas FigmaCanvas,
			SvgFileInfo SvgFileInfo,
			FigmaAltseedNode FigmaAltseedNode,
			PngFileInfo PngFileInfo);
	}

	[AttributeUsage(AttributeTargets.Constructor)]
	internal class PipelineInputAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	internal class PipelineOutputAttribute : Attribute
	{
		public string ResourceTag { get; }

		public PipelineOutputAttribute(string resourceTag)
		{
			ResourceTag = resourceTag;
		}
	}
}
