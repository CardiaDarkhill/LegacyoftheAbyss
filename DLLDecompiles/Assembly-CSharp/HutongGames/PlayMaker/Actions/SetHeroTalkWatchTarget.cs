using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F6 RID: 4598
	public class SetHeroTalkWatchTarget : FsmStateAction
	{
		// Token: 0x06007A7F RID: 31359 RVA: 0x0024C979 File Offset: 0x0024AB79
		public bool HideEndedFacingForwardEvent()
		{
			return this.Target.GetSafe(this);
		}

		// Token: 0x06007A80 RID: 31360 RVA: 0x0024C98C File Offset: 0x0024AB8C
		public override void Reset()
		{
			this.Target = null;
			this.EndedFacingForwardEvent = null;
		}

		// Token: 0x06007A81 RID: 31361 RVA: 0x0024C99C File Offset: 0x0024AB9C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				HeroTalkAnimation.SetWatchTarget(safe.transform, null);
				base.Finish();
				return;
			}
			HeroTalkAnimation.SetWatchTarget(null, new Action(this.End));
		}

		// Token: 0x06007A82 RID: 31362 RVA: 0x0024C9E3 File Offset: 0x0024ABE3
		private void End()
		{
			if (base.Finished)
			{
				return;
			}
			base.Fsm.Event(this.EndedFacingForwardEvent);
			base.Finish();
		}

		// Token: 0x04007AC3 RID: 31427
		public FsmOwnerDefault Target;

		// Token: 0x04007AC4 RID: 31428
		[HideIf("HideEndedFacingForwardEvent")]
		public FsmEvent EndedFacingForwardEvent;
	}
}
