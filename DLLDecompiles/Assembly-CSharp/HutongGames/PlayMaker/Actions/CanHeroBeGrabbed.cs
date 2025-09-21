using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001243 RID: 4675
	public class CanHeroBeGrabbed : FsmStateAction
	{
		// Token: 0x06007BA9 RID: 31657 RVA: 0x0025024B File Offset: 0x0024E44B
		public override void Reset()
		{
			this.eventTarget = null;
			this.canTakeDmgEvent = null;
			this.cannotTakeDmgEvent = null;
		}

		// Token: 0x06007BAA RID: 31658 RVA: 0x00250264 File Offset: 0x0024E464
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance.CanBeGrabbed() && !instance.WillDoBellBindHit(true))
			{
				base.Fsm.Event(this.eventTarget, this.canTakeDmgEvent);
			}
			else
			{
				base.Fsm.Event(this.eventTarget, this.cannotTakeDmgEvent);
			}
			base.Finish();
		}

		// Token: 0x04007BE2 RID: 31714
		public FsmEventTarget eventTarget;

		// Token: 0x04007BE3 RID: 31715
		public FsmEvent canTakeDmgEvent;

		// Token: 0x04007BE4 RID: 31716
		public FsmEvent cannotTakeDmgEvent;
	}
}
