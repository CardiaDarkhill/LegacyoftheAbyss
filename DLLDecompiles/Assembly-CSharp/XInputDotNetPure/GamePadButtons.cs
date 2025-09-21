using System;

namespace XInputDotNetPure
{
	// Token: 0x020008D2 RID: 2258
	public struct GamePadButtons
	{
		// Token: 0x06004EEE RID: 20206 RVA: 0x0016F280 File Offset: 0x0016D480
		internal GamePadButtons(ButtonState start, ButtonState back, ButtonState leftStick, ButtonState rightStick, ButtonState leftShoulder, ButtonState rightShoulder, ButtonState a, ButtonState b, ButtonState x, ButtonState y)
		{
			this.start = start;
			this.back = back;
			this.leftStick = leftStick;
			this.rightStick = rightStick;
			this.leftShoulder = leftShoulder;
			this.rightShoulder = rightShoulder;
			this.a = a;
			this.b = b;
			this.x = x;
			this.y = y;
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06004EEF RID: 20207 RVA: 0x0016F2DA File Offset: 0x0016D4DA
		public ButtonState Start
		{
			get
			{
				return this.start;
			}
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06004EF0 RID: 20208 RVA: 0x0016F2E2 File Offset: 0x0016D4E2
		public ButtonState Back
		{
			get
			{
				return this.back;
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06004EF1 RID: 20209 RVA: 0x0016F2EA File Offset: 0x0016D4EA
		public ButtonState LeftStick
		{
			get
			{
				return this.leftStick;
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06004EF2 RID: 20210 RVA: 0x0016F2F2 File Offset: 0x0016D4F2
		public ButtonState RightStick
		{
			get
			{
				return this.rightStick;
			}
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06004EF3 RID: 20211 RVA: 0x0016F2FA File Offset: 0x0016D4FA
		public ButtonState LeftShoulder
		{
			get
			{
				return this.leftShoulder;
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06004EF4 RID: 20212 RVA: 0x0016F302 File Offset: 0x0016D502
		public ButtonState RightShoulder
		{
			get
			{
				return this.rightShoulder;
			}
		}

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06004EF5 RID: 20213 RVA: 0x0016F30A File Offset: 0x0016D50A
		public ButtonState A
		{
			get
			{
				return this.a;
			}
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06004EF6 RID: 20214 RVA: 0x0016F312 File Offset: 0x0016D512
		public ButtonState B
		{
			get
			{
				return this.b;
			}
		}

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06004EF7 RID: 20215 RVA: 0x0016F31A File Offset: 0x0016D51A
		public ButtonState X
		{
			get
			{
				return this.x;
			}
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06004EF8 RID: 20216 RVA: 0x0016F322 File Offset: 0x0016D522
		public ButtonState Y
		{
			get
			{
				return this.y;
			}
		}

		// Token: 0x04004F7E RID: 20350
		private ButtonState start;

		// Token: 0x04004F7F RID: 20351
		private ButtonState back;

		// Token: 0x04004F80 RID: 20352
		private ButtonState leftStick;

		// Token: 0x04004F81 RID: 20353
		private ButtonState rightStick;

		// Token: 0x04004F82 RID: 20354
		private ButtonState leftShoulder;

		// Token: 0x04004F83 RID: 20355
		private ButtonState rightShoulder;

		// Token: 0x04004F84 RID: 20356
		private ButtonState a;

		// Token: 0x04004F85 RID: 20357
		private ButtonState b;

		// Token: 0x04004F86 RID: 20358
		private ButtonState x;

		// Token: 0x04004F87 RID: 20359
		private ButtonState y;
	}
}
