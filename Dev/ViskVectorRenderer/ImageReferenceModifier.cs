using System.Collections.Generic;
using System.Linq;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;

namespace ViskVectorRenderer
{
	internal class ImageReferenceModifier
	{
		public IEnumerable<GroupRenderResult> Apply(IEnumerable<GroupRenderResult> groupRenderResults)
		{
			foreach (var group in groupRenderResults)
			{
				if (group is ImageGroupRenderResult image)
				{
					var modifiedElements = group.Elements
						.Select(
							element =>
							{
								var list = new List<ICapability>(element.Capabilities);
								list.Add(new Image(image.FilePath));
								list.RemoveAll(x => x is RoundedRectangle);
								list.RemoveAll(x => x is Paint);
								return element with { Capabilities = list.ToArray() };
							})
						.ToArray();
					yield return new ImageGroupRenderResult(
						modifiedElements, image.SvgDocument, image.FilePath);
				}
				else
				{
					yield return group;
				}
			}
		}
	}
}
