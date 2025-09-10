using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012EC RID: 4844
	[ActionCategory("Hollow Knight")]
	public class SilkCloudNextOrb : FsmStateAction
	{
		// Token: 0x06007E3E RID: 32318 RVA: 0x00258884 File Offset: 0x00256A84
		public override void Reset()
		{
			this.nextOrb = null;
			this.eventTarget = null;
			this.finalOrbEvent = null;
		}

		// Token: 0x06007E3F RID: 32319 RVA: 0x0025889C File Offset: 0x00256A9C
		public override void OnEnter()
		{
			SilkflyCloud component = base.Owner.GetComponent<SilkflyCloud>();
			if (component.IsFinalOrb())
			{
				base.Fsm.Event(this.eventTarget, this.finalOrbEvent);
			}
			else if (!this.nextOrb.IsNone)
			{
				this.nextOrb.Value = component.GetNextOrb();
			}
			base.Finish();
		}

		// Token: 0x04007E0D RID: 32269
		[UIHint(UIHint.Variable)]
		public FsmGameObject nextOrb;

		// Token: 0x04007E0E RID: 32270
		[UIHint(UIHint.Variable)]
		public FsmEventTarget eventTarget;

		// Token: 0x04007E0F RID: 32271
		public string finalOrbEvent;
	}
}
