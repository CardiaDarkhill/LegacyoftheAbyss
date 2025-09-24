using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001244 RID: 4676
	public class CanHeroBeGrabbedV2 : FsmStateAction
	{
		// Token: 0x06007BAC RID: 31660 RVA: 0x002502C6 File Offset: 0x0024E4C6
		public override void Reset()
		{
			this.eventTarget = null;
			this.canTakeDmgEvent = null;
			this.cannotTakeDmgEvent = null;
			this.ignoreParryState = null;
		}

		// Token: 0x06007BAD RID: 31661 RVA: 0x002502E4 File Offset: 0x0024E4E4
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance.CanBeGrabbed(this.ignoreParryState.Value) && (!this.ignoreBellBind.Value || !instance.WillDoBellBindHit(this.triggerBellBindEffect.Value)))
			{
				base.Fsm.Event(this.eventTarget, this.canTakeDmgEvent);
			}
			else
			{
				base.Fsm.Event(this.eventTarget, this.cannotTakeDmgEvent);
			}
			base.Finish();
		}

		// Token: 0x04007BE5 RID: 31717
		public FsmEventTarget eventTarget;

		// Token: 0x04007BE6 RID: 31718
		public FsmEvent canTakeDmgEvent;

		// Token: 0x04007BE7 RID: 31719
		public FsmEvent cannotTakeDmgEvent;

		// Token: 0x04007BE8 RID: 31720
		public FsmBool ignoreParryState;

		// Token: 0x04007BE9 RID: 31721
		public FsmBool ignoreBellBind;

		// Token: 0x04007BEA RID: 31722
		public FsmBool triggerBellBindEffect;
	}
}
