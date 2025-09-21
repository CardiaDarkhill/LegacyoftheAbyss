using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200126B RID: 4715
	public class CanHeroTakeDamageIgnoreInvul : FsmStateAction
	{
		// Token: 0x06007C5C RID: 31836 RVA: 0x00252F0C File Offset: 0x0025110C
		public override void Reset()
		{
			this.eventTarget = null;
			this.canTakeDmgEvent = null;
			this.cannotTakeDmgEvent = null;
		}

		// Token: 0x06007C5D RID: 31837 RVA: 0x00252F24 File Offset: 0x00251124
		public override void OnEnter()
		{
			if (HeroController.instance.CanTakeDamageIgnoreInvul())
			{
				base.Fsm.Event(this.eventTarget, this.canTakeDmgEvent);
			}
			else
			{
				base.Fsm.Event(this.eventTarget, this.cannotTakeDmgEvent);
			}
			base.Finish();
		}

		// Token: 0x04007C6C RID: 31852
		public FsmEventTarget eventTarget;

		// Token: 0x04007C6D RID: 31853
		public FsmEvent canTakeDmgEvent;

		// Token: 0x04007C6E RID: 31854
		public FsmEvent cannotTakeDmgEvent;
	}
}
