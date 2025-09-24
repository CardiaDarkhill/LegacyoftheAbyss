using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008AC RID: 2220
	public static class PlaystationLogHandler
	{
		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06004CC7 RID: 19655 RVA: 0x00168A5D File Offset: 0x00166C5D
		// (set) Token: 0x06004CC8 RID: 19656 RVA: 0x00168A75 File Offset: 0x00166C75
		public static IMessagePrinter Printer
		{
			get
			{
				if (PlaystationLogHandler.printer == null)
				{
					PlaystationLogHandler.printer = new DummyPrint();
				}
				return PlaystationLogHandler.printer;
			}
			set
			{
				PlaystationLogHandler.printer = value;
			}
		}

		// Token: 0x06004CC9 RID: 19657 RVA: 0x00168A7D File Offset: 0x00166C7D
		public static void Log(object message)
		{
			PlaystationLogHandler.LogMessage(message, Color.white, Message.MessageType.Log);
		}

		// Token: 0x06004CCA RID: 19658 RVA: 0x00168A8B File Offset: 0x00166C8B
		public static void LogWarning(object message)
		{
			PlaystationLogHandler.LogMessage(message, Color.yellow, Message.MessageType.Warning);
		}

		// Token: 0x06004CCB RID: 19659 RVA: 0x00168A99 File Offset: 0x00166C99
		public static void LogError(object message)
		{
			PlaystationLogHandler.LogMessage(message, Color.red, Message.MessageType.Error);
		}

		// Token: 0x06004CCC RID: 19660 RVA: 0x00168AA7 File Offset: 0x00166CA7
		public static void Log(object message, Object context)
		{
			PlaystationLogHandler.LogMessage(message, Color.white, Message.MessageType.Log, context);
		}

		// Token: 0x06004CCD RID: 19661 RVA: 0x00168AB6 File Offset: 0x00166CB6
		public static void LogWarning(object message, Object context)
		{
			PlaystationLogHandler.LogMessage(message, Color.yellow, Message.MessageType.Warning, context);
		}

		// Token: 0x06004CCE RID: 19662 RVA: 0x00168AC5 File Offset: 0x00166CC5
		public static void LogError(object message, Object context)
		{
			PlaystationLogHandler.LogMessage(message, Color.red, Message.MessageType.Error, context);
		}

		// Token: 0x06004CCF RID: 19663 RVA: 0x00168AD4 File Offset: 0x00166CD4
		public static void LogMessage(object message, Color color, Message.MessageType messageType = Message.MessageType.Log)
		{
			PlaystationLogHandler.PrintMessageWithStackTrace(new Message(message.ToString(), color, messageType));
		}

		// Token: 0x06004CD0 RID: 19664 RVA: 0x00168AE8 File Offset: 0x00166CE8
		public static void LogMessage(object message, Color color, Message.MessageType messageType, Object context)
		{
			if (UnityThreadContext.IsUnityMainThread)
			{
				PlaystationLogHandler.PrintMessageWithStackTrace(new Message(message.ToString(), color, messageType, context));
				return;
			}
			CoreLoop.InvokeSafe(delegate
			{
				PlaystationLogHandler.PrintMessage(new Message(message.ToString(), color, messageType, context));
			});
		}

		// Token: 0x06004CD1 RID: 19665 RVA: 0x00168B57 File Offset: 0x00166D57
		private static void PrintMessage(Message message)
		{
			PlaystationLogHandler.Printer.PrintMessage(message);
		}

		// Token: 0x06004CD2 RID: 19666 RVA: 0x00168B64 File Offset: 0x00166D64
		private static void PrintMessageWithStackTrace(Message message)
		{
			PlaystationLogHandler.Printer.PrintMessage(message);
			switch (message.messageType)
			{
			case Message.MessageType.Log:
				if (message.hasContext)
				{
					Debug.Log(message.message, message.context);
					return;
				}
				Debug.Log(message.message);
				return;
			case Message.MessageType.Warning:
				if (message.hasContext)
				{
					Debug.LogWarning(message.message, message.context);
					return;
				}
				Debug.LogWarning(message.message);
				return;
			case Message.MessageType.Error:
				if (message.hasContext)
				{
					Debug.LogError(message.message, message.context);
					return;
				}
				Debug.LogError(message.message);
				return;
			default:
				return;
			}
		}

		// Token: 0x04004DDC RID: 19932
		private static IMessagePrinter printer;
	}
}
