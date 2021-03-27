using System;
using System.Diagnostics.CodeAnalysis;

namespace ViskVectorRenderer
{
	internal class StartupOption
	{
		public string? PackagePath { get; set; }
		public string? OutputPath { get; set; }

		[MemberNotNull(nameof(PackagePath), nameof(OutputPath))]
		public void CheckOption()
		{
			if (PackagePath is null || OutputPath is null)
			{
				throw new InvalidOperationException(
					$"Usase:\n{Program.AppName} {nameof(PackagePath)}=\"Path to package\" {nameof(OutputPath)}=\"Path to output package\"");
			}
		}
	}
}
