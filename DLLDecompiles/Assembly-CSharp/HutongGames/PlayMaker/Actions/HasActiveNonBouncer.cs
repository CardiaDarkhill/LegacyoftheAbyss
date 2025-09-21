using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012FB RID: 4859
	[Tooltip("Checks if object has an active non bouncer.")]
	public sealed class HasActiveNonBouncer : FsmStateAction
	{
		// Token: 0x06007E6C RID: 32364 RVA: 0x00258E51 File Offset: 0x00257051
		public override void Reset()
		{
			this.Target = null;
			this.requireActive = true;
			this.trueEvent = null;
			this.falseEvent = null;
		}

		// Token: 0x06007E6D RID: 32365 RVA: 0x00258E74 File Offset: 0x00257074
		public override void OnEnter()
		{
			NonBouncer safe = this.Target.GetSafe(this);
			if (safe != null && (safe.active || !this.requireActive.Value))
			{
				base.Fsm.Event(this.trueEvent);
				base.Finish();
				return;
			}
			base.Fsm.Event(this.falseEvent);
			base.Finish();
		}

		// Token: 0x04007E26 RID: 32294
		[RequiredField]
		[CheckForComponent(typeof(NonBouncer))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E27 RID: 32295
		public FsmBool requireActive;

		// Token: 0x04007E28 RID: 32296
		public FsmEvent trueEvent;

		// Token: 0x04007E29 RID: 32297
		public FsmEvent falseEvent;
	}
}
