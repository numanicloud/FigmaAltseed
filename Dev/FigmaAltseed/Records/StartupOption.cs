using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigmaAltseed
{
	internal class StartupOption
	{
		public string? FileId { get; set; }
		public string? Token { get; set; }

		public bool GetIsValid()
		{
			return FileId is not null && Token is not null;
		}
	}
}
