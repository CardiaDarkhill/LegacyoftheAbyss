using System;

namespace XInputDotNetPure
{
	// Token: 0x020008D3 RID: 2259
	public struct GamePadDPad
	{
		// Token: 0x06004EF9 RID: 20217 RVA: 0x0016F32A File Offset: 0x0016D52A
		internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
		{
			this.up = up;
			this.down = down;
			this.left = left;
			this.right = right;
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06004EFA RID: 20218 RVA: 0x0016F349 File Offset: 0x0016D549
		public ButtonState Up
		{
			get
			{
				return this.up;
			}
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06004EFB RID: 20219 RVA: 0x0016F351 File Offset: 0x0016D551
		public ButtonState Down
		{
			get
			{
				return this.down;
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06004EFC RID: 20220 RVA: 0x0016F359 File Offset: 0x0016D559
		public ButtonState Left
		{
			get
			{
				return this.left;
			}
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06004EFD RID: 20221 RVA: 0x0016F361 File Offset: 0x0016D561
		public ButtonState Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04004F88 RID: 20360
		private ButtonState up;

		// Token: 0x04004F89 RID: 20361
		private ButtonState down;

		// Token: 0x04004F8A RID: 20362
		private ButtonState left;

		// Token: 0x04004F8B RID: 20363
		private ButtonState right;
	}
}
