using FigmaSharp.Models;

namespace FigmaAltseed.Common
{
	internal static class Conventions
	{
		public const string AltTransformTag = "@AltTransform";

		public static bool IsAltTransform(this FigmaNode node)
		{
			return node.name.EndsWith(AltTransformTag);
		}

		public static string RemoveAltTransformTag(this string name)
		{
			return name.Replace(AltTransformTag, "");
		}
	}
}
