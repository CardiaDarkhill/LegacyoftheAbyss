using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200124B RID: 4683
	public class HeroWallJumpBrollyCheck : FsmStateAction
	{
		// Token: 0x06007BD0 RID: 31696 RVA: 0x00250833 File Offset: 0x0024EA33
		public override void Reset()
		{
			this.Target = null;
			this.IsFacingRight = null;
			this.TrueEvent = null;
			this.FalseEvent = null;
		}

		// Token: 0x06007BD1 RID: 31697 RVA: 0x00250854 File Offset: 0x0024EA54
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if (component)
				{
					bool flag = component.IsFacingNearWall(this.IsFacingRight.Value, component.WALLJUMP_BROLLY_RAY_LENGTH, Color.green);
					base.Fsm.Event(flag ? this.TrueEvent : this.FalseEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007BFE RID: 31742
		public FsmOwnerDefault Target;

		// Token: 0x04007BFF RID: 31743
		public FsmBool IsFacingRight;

		// Token: 0x04007C00 RID: 31744
		public FsmEvent TrueEvent;

		// Token: 0x04007C01 RID: 31745
		public FsmEvent FalseEvent;
	}
}
