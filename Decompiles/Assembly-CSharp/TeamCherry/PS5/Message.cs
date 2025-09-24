using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008AB RID: 2219
	public struct Message
	{
		// Token: 0x06004CC5 RID: 19653 RVA: 0x00168A12 File Offset: 0x00166C12
		public Message(string message, Color color, Message.MessageType messageType = Message.MessageType.Log)
		{
			this.message = message;
			this.color = color;
			this.messageType = messageType;
			this.context = null;
			this.hasContext = false;
		}

		// Token: 0x06004CC6 RID: 19654 RVA: 0x00168A37 File Offset: 0x00166C37
		public Message(string message, Color color, Message.MessageType messageType, Object context)
		{
			this.message = message;
			this.color = color;
			this.messageType = messageType;
			this.context = context;
			this.hasContext = true;
		}

		// Token: 0x04004DD7 RID: 19927
		public string message;

		// Token: 0x04004DD8 RID: 19928
		public Color color;

		// Token: 0x04004DD9 RID: 19929
		public Message.MessageType messageType;

		// Token: 0x04004DDA RID: 19930
		public Object context;

		// Token: 0x04004DDB RID: 19931
		public bool hasContext;

		// Token: 0x02001B05 RID: 6917
		public enum MessageType
		{
			// Token: 0x04009B70 RID: 39792
			Log,
			// Token: 0x04009B71 RID: 39793
			Warning,
			// Token: 0x04009B72 RID: 39794
			Error
		}
	}
}
