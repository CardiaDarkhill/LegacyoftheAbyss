using System;
using UnityEngine;

// Token: 0x02000755 RID: 1877
public static class CommandLineArguments
{
	// Token: 0x17000788 RID: 1928
	// (get) Token: 0x06004298 RID: 17048 RVA: 0x00125C1A File Offset: 0x00123E1A
	// (set) Token: 0x06004299 RID: 17049 RVA: 0x00125C21 File Offset: 0x00123E21
	public static bool ShowPerformanceHUD { get; private set; }

	// Token: 0x17000789 RID: 1929
	// (get) Token: 0x0600429A RID: 17050 RVA: 0x00125C29 File Offset: 0x00123E29
	// (set) Token: 0x0600429B RID: 17051 RVA: 0x00125C30 File Offset: 0x00123E30
	public static string RemoteSaveDirectory { get; private set; }

	// Token: 0x1700078A RID: 1930
	// (get) Token: 0x0600429C RID: 17052 RVA: 0x00125C38 File Offset: 0x00123E38
	// (set) Token: 0x0600429D RID: 17053 RVA: 0x00125C3F File Offset: 0x00123E3F
	public static bool EnableDeveloperCheats { get; private set; }

	// Token: 0x0600429E RID: 17054 RVA: 0x00125C48 File Offset: 0x00123E48
	static CommandLineArguments()
	{
		if (Application.isEditor)
		{
			return;
		}
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs == null)
		{
			return;
		}
		foreach (string text in commandLineArgs)
		{
			if (text.Equals("--show-performance-hud", StringComparison.OrdinalIgnoreCase))
			{
				CommandLineArguments.ShowPerformanceHUD = true;
			}
			else if (text.Equals("--enable-developer-cheats", StringComparison.OrdinalIgnoreCase))
			{
				CommandLineArguments.EnableDeveloperCheats = true;
			}
			else if (text.StartsWith("--remote-save-directory=", StringComparison.OrdinalIgnoreCase))
			{
				CommandLineArguments.RemoteSaveDirectory = text.Substring("--remote-save-directory=".Length);
			}
		}
	}

	// Token: 0x04004419 RID: 17433
	private const StringComparison ArgumentComparison = StringComparison.OrdinalIgnoreCase;

	// Token: 0x0400441A RID: 17434
	private const string ShowPerformanceHUDFlag = "--show-performance-hud";

	// Token: 0x0400441C RID: 17436
	private const string RemoteSaveDirectoryPrefix = "--remote-save-directory=";

	// Token: 0x0400441E RID: 17438
	private const string EnableDeveloperCheatsFlag = "--enable-developer-cheats";
}
