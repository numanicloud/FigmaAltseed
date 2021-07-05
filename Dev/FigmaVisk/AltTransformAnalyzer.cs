﻿using System.Collections.Generic;
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
				if (element.GetCapability<FigmaId>() is {} id
					&& id.Name.EndsWith("@AltPosition")
					&& element.GetCapability<BoundingBox>() is {} box)
				{
					var capability = new AltPosition(box.X, box.Y);
					var matchName = id.Name.Replace("@AltPosition", "");

					list.Remove(element);
					alts.Add((element, capability, matchName));
				}
			}

			var replaces = new List<(Element, AltPosition)>();
			foreach (var element in source)
			{
				if (element.GetCapability<AltPosition>() is null
					&& element.GetCapability<FigmaId>() is { } id
					&& alts.FirstOrDefault(k => k.name == id.Name) is {} alt
					&& alt != default)
				{
					list.Remove(element);
					replaces.Add((element, alt.Item2));
				}
			}

			return replaces.Select(x => x.Item1 with
			{
				Capabilities = x.Item1.Capabilities.Append(x.Item2).ToArray()
			}).Concat(list).ToArray();
		}
	}
}
