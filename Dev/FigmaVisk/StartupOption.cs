using System;

namespace FigmaVisk
{
	public class StartupOption
	{
		public string? FileId { get; set; }
		public string? Token { get; set; }

		public void CheckOption()
		{
			if (!(FileId is not null && Token is not null))
			{
				throw new InvalidOperationException(
					$"Usase:\n{Program.AppName}.exe {nameof(FileId)}=\"<FileId>\" {nameof(Token)}=\"<AccessToken>\"");
			}
		}
	}
}
