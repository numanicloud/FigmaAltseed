using Svg;
using Visklusa.Abstraction.Notation;

namespace ViskVectorRenderer
{
	internal abstract class RenderResult
	{
		protected RenderResult(Element element)
		{
			Element = element;
		}

		public Element Element { get; set; }
	}

	internal class SuccessRenderResult : RenderResult
	{
		public SuccessRenderResult(Element element, SvgDocument svgDocument)
			: base(element)
		{
			SvgDocument = svgDocument;
		}

		public SvgDocument SvgDocument { get; set; }
	}

	internal class SkipRenderResult : RenderResult
	{
		public SkipRenderResult(Element element)
			: base(element)
		{
		}
	}
}
