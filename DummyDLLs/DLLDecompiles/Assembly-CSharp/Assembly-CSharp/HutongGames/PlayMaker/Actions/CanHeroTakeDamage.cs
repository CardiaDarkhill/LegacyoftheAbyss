using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001242 RID: 4674
	public class CanHeroTakeDamage : FsmStateAction
	{
		// Token: 0x06007BA6 RID: 31654 RVA: 0x002501DB File Offset: 0x0024E3DB
		public override void Reset()
		{
			this.eventTarget = null;
			this.canTakeDmgEvent = null;
			this.cannotTakeDmgEvent = null;
		}

		// Token: 0x06007BA7 RID: 31655 RVA: 0x002501F4 File Offset: 0x0024E3F4
		public override void OnEnter()
		{
			if (HeroController.instance.CanTakeDamage())
			{
				base.Fsm.Event(this.eventTarget, this.canTakeDmgEvent);
			}
			else
			{
				base.Fsm.Event(this.eventTarget, this.cannotTakeDmgEvent);
			}
			base.Finish();
		}

		// Token: 0x04007BDF RID: 31711
		public FsmEventTarget eventTarget;

		// Token: 0x04007BE0 RID: 31712
		public FsmEvent canTakeDmgEvent;

		// Token: 0x04007BE1 RID: 31713
		public FsmEvent cannotTakeDmgEvent;
	}
}
