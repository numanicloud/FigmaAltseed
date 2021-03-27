using Svg;
using Visklusa.Abstraction.Notation;

namespace ViskVectorRenderer
{
	internal abstract record RenderResult(Element Element);

	internal record SuccessRenderResult(Element Element, SvgDocument SvgDocument, string FilePath)
		: RenderResult(Element);

	internal record SkipRenderResult(Element Element) : RenderResult(Element);
}
