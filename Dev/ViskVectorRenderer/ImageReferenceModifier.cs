using System.Collections.Generic;
using System.Linq;
using FigmaVisk.Capability;

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
								var newCaps = element.Capabilities
									.Append(new Image(image.FilePath))
									.ToArray();
								return element with { Capabilities = newCaps };
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
