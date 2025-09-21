using System;

namespace InControl
{
	// Token: 0x02000900 RID: 2304
	public static class Logger
	{
		// Token: 0x140000F9 RID: 249
		// (add) Token: 0x060050CD RID: 20685 RVA: 0x00173A78 File Offset: 0x00171C78
		// (remove) Token: 0x060050CE RID: 20686 RVA: 0x00173AAC File Offset: 0x00171CAC
		public static event Action<LogMessage> OnLogMessage;

		// Token: 0x060050CF RID: 20687 RVA: 0x00173AE0 File Offset: 0x00171CE0
		public static void LogInfo(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage obj = new LogMessage
				{
					Text = text,
					Type = LogMessageType.Info
				};
				Logger.OnLogMessage(obj);
			}
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x00173B1C File Offset: 0x00171D1C
		public static void LogWarning(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage obj = new LogMessage
				{
					Text = text,
					Type = LogMessageType.Warning
				};
				Logger.OnLogMessage(obj);
			}
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x00173B58 File Offset: 0x00171D58
		public static void LogError(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage obj = new LogMessage
				{
					Text = text,
					Type = LogMessageType.Error
				};
				Logger.OnLogMessage(obj);
			}
		}
	}
}
