using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F7 RID: 4855
	public class IsRefillSoundSuppressed : FsmStateAction
	{
		// Token: 0x06007E5E RID: 32350 RVA: 0x00258C53 File Offset: 0x00256E53
		public override void Reset()
		{
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeValue = null;
			this.everyFrame = null;
		}

		// Token: 0x06007E5F RID: 32351 RVA: 0x00258C74 File Offset: 0x00256E74
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null && instance.IsRefillSoundsSuppressed)
			{
				base.Fsm.Event(this.trueEvent);
				this.storeValue.Value = true;
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
				this.storeValue.Value = false;
			}
			if (!this.everyFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x04007E1C RID: 32284
		public FsmEvent trueEvent;

		// Token: 0x04007E1D RID: 32285
		public FsmEvent falseEvent;

		// Token: 0x04007E1E RID: 32286
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		// Token: 0x04007E1F RID: 32287
		public FsmBool everyFrame;
	}
}
