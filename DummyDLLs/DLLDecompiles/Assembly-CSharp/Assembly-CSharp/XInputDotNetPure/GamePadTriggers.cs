using System;

namespace XInputDotNetPure
{
	// Token: 0x020008D5 RID: 2261
	public struct GamePadTriggers
	{
		// Token: 0x06004F01 RID: 20225 RVA: 0x0016F389 File Offset: 0x0016D589
		internal GamePadTriggers(float left, float right)
		{
			this.left = left;
			this.right = right;
		}

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06004F02 RID: 20226 RVA: 0x0016F399 File Offset: 0x0016D599
		public float Left
		{
			get
			{
				return this.left;
			}
		}

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06004F03 RID: 20227 RVA: 0x0016F3A1 File Offset: 0x0016D5A1
		public float Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04004F8E RID: 20366
		private float left;

		// Token: 0x04004F8F RID: 20367
		private float right;
	}
}
