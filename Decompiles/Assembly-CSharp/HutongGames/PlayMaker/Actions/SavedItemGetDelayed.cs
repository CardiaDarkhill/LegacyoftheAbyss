using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200128E RID: 4750
	public class SavedItemGetDelayed : FsmStateAction
	{
		// Token: 0x06007CD4 RID: 31956 RVA: 0x00254885 File Offset: 0x00252A85
		public override void Reset()
		{
			this.Item = null;
			this.Delay = null;
		}

		// Token: 0x06007CD5 RID: 31957 RVA: 0x00254895 File Offset: 0x00252A95
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.Delay.Value <= 0f)
			{
				this.DoGet();
			}
		}

		// Token: 0x06007CD6 RID: 31958 RVA: 0x002548BA File Offset: 0x00252ABA
		public override void OnUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.Delay.Value)
			{
				this.DoGet();
			}
		}

		// Token: 0x06007CD7 RID: 31959 RVA: 0x002548E8 File Offset: 0x00252AE8
		private void DoGet()
		{
			SavedItem savedItem = this.Item.Value as SavedItem;
			if (savedItem != null)
			{
				savedItem.Get(true);
			}
			base.Finish();
		}

		// Token: 0x06007CD8 RID: 31960 RVA: 0x0025491C File Offset: 0x00252B1C
		public override float GetProgress()
		{
			if (this.Delay.Value <= 0f)
			{
				return 0f;
			}
			return this.timer / this.Delay.Value;
		}

		// Token: 0x04007CEA RID: 31978
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		// Token: 0x04007CEB RID: 31979
		public FsmFloat Delay;

		// Token: 0x04007CEC RID: 31980
		private float timer;
	}
}
