using System;

namespace InControl
{
	// Token: 0x020008FD RID: 2301
	[Serializable]
	public class InControlException : Exception
	{
		// Token: 0x060050CA RID: 20682 RVA: 0x00173A5B File Offset: 0x00171C5B
		public InControlException()
		{
		}

		// Token: 0x060050CB RID: 20683 RVA: 0x00173A63 File Offset: 0x00171C63
		public InControlException(string message) : base(message)
		{
		}

		// Token: 0x060050CC RID: 20684 RVA: 0x00173A6C File Offset: 0x00171C6C
		public InControlException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
