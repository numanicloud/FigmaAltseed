using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FigmaAltseed.Common;
using Svg.Exceptions;

namespace FigmaAltseed.Converter.Steps
{
	internal record PngFileInfo(Bitmap Bitmap, string Path);

	internal class SvgToPng
	{
		public IEnumerable<PngFileInfo> Covert(IEnumerable<SvgFileInfo> svgDocuments)
		{
			return svgDocuments.Select(x =>
			{
				try
				{
					return new PngFileInfo(x.Document.Draw(), x.FilePath);
				}
				catch (SvgMemoryException exception)
				{
					Console.WriteLine($"{exception}; {x.FilePath}");
					return null;
				}
			}).FilterNull();
		}
	}
}
