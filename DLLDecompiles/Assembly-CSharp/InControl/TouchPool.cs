using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200092C RID: 2348
	public class TouchPool
	{
		// Token: 0x0600535B RID: 21339 RVA: 0x0017D53C File Offset: 0x0017B73C
		public TouchPool(int capacity)
		{
			this.freeTouches = new List<Touch>(capacity);
			for (int i = 0; i < capacity; i++)
			{
				this.freeTouches.Add(new Touch());
			}
			this.usedTouches = new List<Touch>(capacity);
			this.Touches = new ReadOnlyCollection<Touch>(this.usedTouches);
		}

		// Token: 0x0600535C RID: 21340 RVA: 0x0017D594 File Offset: 0x0017B794
		public TouchPool() : this(16)
		{
		}

		// Token: 0x0600535D RID: 21341 RVA: 0x0017D5A0 File Offset: 0x0017B7A0
		public Touch FindOrCreateTouch(int fingerId)
		{
			int count = this.usedTouches.Count;
			Touch touch;
			for (int i = 0; i < count; i++)
			{
				touch = this.usedTouches[i];
				if (touch.fingerId == fingerId)
				{
					return touch;
				}
			}
			touch = this.NewTouch();
			touch.fingerId = fingerId;
			this.usedTouches.Add(touch);
			return touch;
		}

		// Token: 0x0600535E RID: 21342 RVA: 0x0017D5F8 File Offset: 0x0017B7F8
		public Touch FindTouch(int fingerId)
		{
			int count = this.usedTouches.Count;
			for (int i = 0; i < count; i++)
			{
				Touch touch = this.usedTouches[i];
				if (touch.fingerId == fingerId)
				{
					return touch;
				}
			}
			return null;
		}

		// Token: 0x0600535F RID: 21343 RVA: 0x0017D638 File Offset: 0x0017B838
		private Touch NewTouch()
		{
			int count = this.freeTouches.Count;
			if (count > 0)
			{
				Touch result = this.freeTouches[count - 1];
				this.freeTouches.RemoveAt(count - 1);
				return result;
			}
			return new Touch();
		}

		// Token: 0x06005360 RID: 21344 RVA: 0x0017D677 File Offset: 0x0017B877
		public void FreeTouch(Touch touch)
		{
			touch.Reset();
			this.freeTouches.Add(touch);
		}

		// Token: 0x06005361 RID: 21345 RVA: 0x0017D68C File Offset: 0x0017B88C
		public void FreeEndedTouches()
		{
			for (int i = this.usedTouches.Count - 1; i >= 0; i--)
			{
				if (this.usedTouches[i].phase == TouchPhase.Ended)
				{
					this.usedTouches.RemoveAt(i);
				}
			}
		}

		// Token: 0x0400535D RID: 21341
		public readonly ReadOnlyCollection<Touch> Touches;

		// Token: 0x0400535E RID: 21342
		private List<Touch> usedTouches;

		// Token: 0x0400535F RID: 21343
		private List<Touch> freeTouches;
	}
}
