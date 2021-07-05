using System.Collections.Generic;
using System.Linq;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;

namespace FigmaVisk
{
	internal class AltTransformAnalyzer
	{
		public Element[] Convert(Element[] source)
		{
			var list = new List<Element>(source);
			var alts = new List<(Element, AltPosition, string name)>();
			foreach (var element in source)
			{
				if (element.GetCapability<AltPosition>() is {} alt
					&& element.GetCapability<FigmaId>() is {} id)
				{
					list.Remove(element);
					alts.Add((element, alt, id.Name));
				}
			}

			var replaces = new List<(Element, AltPosition)>();
			foreach (var item in source)
			{
				if (item.GetCapability<AltPosition>() is null
					&& item.GetCapability<FigmaId>() is { } id
					&& alts.FirstOrDefault(k => k.name == id.Name) is {} alt)
				{
					replaces.Add((item, alt.Item2));
				}
			}

			return replaces.Select(x => x.Item1 with
			{
				Capabilities = x.Item1.Capabilities.Append(x.Item2).ToArray()
			}).Concat(list).ToArray();
		}
	}
}
