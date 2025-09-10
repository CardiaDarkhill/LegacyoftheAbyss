using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A6 RID: 2214
	public abstract class PSInputBase
	{
		// Token: 0x06004C91 RID: 19601 RVA: 0x00168475 File Offset: 0x00166675
		public static PSInputBase GetPSInput()
		{
			return new PS5InControlInput();
		}

		// Token: 0x06004C92 RID: 19602
		public abstract void Init(GamePad gamePad);

		// Token: 0x06004C93 RID: 19603
		public abstract Vector2 GetThumbStickLeft();

		// Token: 0x06004C94 RID: 19604
		public abstract Vector2 GetThumbStickRight();

		// Token: 0x06004C95 RID: 19605
		public abstract bool GetL3();

		// Token: 0x06004C96 RID: 19606
		public abstract bool GetR3();

		// Token: 0x06004C97 RID: 19607
		public abstract bool GetOptions();

		// Token: 0x06004C98 RID: 19608
		public abstract bool GetCross();

		// Token: 0x06004C99 RID: 19609
		public abstract bool GetCircle();

		// Token: 0x06004C9A RID: 19610
		public abstract bool GetSquare();

		// Token: 0x06004C9B RID: 19611
		public abstract bool GetTriangle();

		// Token: 0x06004C9C RID: 19612
		public abstract bool GetDpadRight();

		// Token: 0x06004C9D RID: 19613
		public abstract bool GetDpadLeft();

		// Token: 0x06004C9E RID: 19614
		public abstract bool GetDpadUp();

		// Token: 0x06004C9F RID: 19615
		public abstract bool GetDpadDown();

		// Token: 0x06004CA0 RID: 19616
		public abstract bool GetR1();

		// Token: 0x06004CA1 RID: 19617
		public abstract bool GetR2();

		// Token: 0x06004CA2 RID: 19618
		public abstract bool GetL1();

		// Token: 0x06004CA3 RID: 19619
		public abstract bool GetL2();

		// Token: 0x06004CA4 RID: 19620
		public abstract bool TouchPadButton();
	}
}
