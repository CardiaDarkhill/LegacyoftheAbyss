using System;
using UnityEngine;

namespace XInputDotNetPure
{
	// Token: 0x020008D4 RID: 2260
	public struct GamePadThumbSticks
	{
		// Token: 0x06004EFE RID: 20222 RVA: 0x0016F369 File Offset: 0x0016D569
		internal GamePadThumbSticks(GamePadThumbSticks.StickValue left, GamePadThumbSticks.StickValue right)
		{
			this.left = left;
			this.right = right;
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06004EFF RID: 20223 RVA: 0x0016F379 File Offset: 0x0016D579
		public GamePadThumbSticks.StickValue Left
		{
			get
			{
				return this.left;
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06004F00 RID: 20224 RVA: 0x0016F381 File Offset: 0x0016D581
		public GamePadThumbSticks.StickValue Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04004F8C RID: 20364
		private GamePadThumbSticks.StickValue left;

		// Token: 0x04004F8D RID: 20365
		private GamePadThumbSticks.StickValue right;

		// Token: 0x02001B58 RID: 7000
		public struct StickValue
		{
			// Token: 0x060099F2 RID: 39410 RVA: 0x002B2847 File Offset: 0x002B0A47
			internal StickValue(float x, float y)
			{
				this.vector = new Vector2(x, y);
			}

			// Token: 0x1700119B RID: 4507
			// (get) Token: 0x060099F3 RID: 39411 RVA: 0x002B2856 File Offset: 0x002B0A56
			public float X
			{
				get
				{
					return this.vector.x;
				}
			}

			// Token: 0x1700119C RID: 4508
			// (get) Token: 0x060099F4 RID: 39412 RVA: 0x002B2863 File Offset: 0x002B0A63
			public float Y
			{
				get
				{
					return this.vector.y;
				}
			}

			// Token: 0x1700119D RID: 4509
			// (get) Token: 0x060099F5 RID: 39413 RVA: 0x002B2870 File Offset: 0x002B0A70
			public Vector2 Vector
			{
				get
				{
					return this.vector;
				}
			}

			// Token: 0x04009C64 RID: 40036
			private Vector2 vector;
		}
	}
}
