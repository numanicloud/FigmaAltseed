using Svg;
using Visklusa.Abstraction.Notation;

namespace ViskVectorRenderer
{
	internal abstract record GroupRenderResult(Element[] Elements);

	internal record ImageGroupRenderResult
		(Element[] Elements, SvgDocument SvgDocument, string FilePath) 
		: GroupRenderResult(Elements);

	internal record SkipGroupRenderResult(Element[] Elements) : GroupRenderResult(Elements);
}
