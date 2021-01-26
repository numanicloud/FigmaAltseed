using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FigmaAltseed
{
	internal record PngFileInfo(Bitmap Bitmap, string Path);

	internal class SvgToPng
	{
		public IEnumerable<PngFileInfo> Covert(IEnumerable<SvgFileInfo> svgDocuments)
		{
			return svgDocuments.Select(x => new PngFileInfo(x.Document.Draw(), x.FilePath));
		}
	}
}
