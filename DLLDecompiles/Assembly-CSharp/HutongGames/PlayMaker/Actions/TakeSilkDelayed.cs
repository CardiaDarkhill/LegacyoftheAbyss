using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200123A RID: 4666
	[ActionCategory("Hollow Knight")]
	public class TakeSilkDelayed : FsmStateAction
	{
		// Token: 0x06007B7F RID: 31615 RVA: 0x0024FC1A File Offset: 0x0024DE1A
		public override void Reset()
		{
			this.Amount = null;
			this.Delay = null;
			this.TakeSource = null;
		}

		// Token: 0x06007B80 RID: 31616 RVA: 0x0024FC31 File Offset: 0x0024DE31
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.Delay.Value <= 0f)
			{
				this.TakeSilk();
			}
		}

		// Token: 0x06007B81 RID: 31617 RVA: 0x0024FC56 File Offset: 0x0024DE56
		public override void OnUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.Delay.Value)
			{
				this.TakeSilk();
			}
		}

		// Token: 0x06007B82 RID: 31618 RVA: 0x0024FC84 File Offset: 0x0024DE84
		public void TakeSilk()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.TakeSilk(this.Amount.Value, (SilkSpool.SilkTakeSource)this.TakeSource.Value);
			}
			base.Finish();
		}

		// Token: 0x04007BB8 RID: 31672
		public FsmInt Amount;

		// Token: 0x04007BB9 RID: 31673
		public FsmFloat Delay;

		// Token: 0x04007BBA RID: 31674
		[ObjectType(typeof(SilkSpool.SilkTakeSource))]
		public FsmEnum TakeSource;

		// Token: 0x04007BBB RID: 31675
		private float timer;
	}
}
