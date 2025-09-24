using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D0 RID: 4816
	[ActionCategory("Enemy AI")]
	public class WallClingerGetOptions : FsmStateAction
	{
		// Token: 0x06007DBA RID: 32186 RVA: 0x0025729D File Offset: 0x0025549D
		public override void Reset()
		{
			this.Target = null;
			this.clinger = null;
			this.MoveSpeed = null;
			this.ClimbUpAnim = null;
			this.ClimbDownAnim = null;
		}

		// Token: 0x06007DBB RID: 32187 RVA: 0x002572C4 File Offset: 0x002554C4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.clinger = safe.GetComponent<WallClinger>();
			}
			if (!this.clinger)
			{
				base.Finish();
				return;
			}
			this.MoveSpeed.Value = this.clinger.MoveSpeed;
			this.ClimbUpAnim.Value = this.clinger.ClimbUpAnim;
			this.ClimbDownAnim.Value = this.clinger.ClimbDownAnim;
			base.Finish();
		}

		// Token: 0x04007DA1 RID: 32161
		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007DA2 RID: 32162
		private WallClinger clinger;

		// Token: 0x04007DA3 RID: 32163
		[UIHint(UIHint.Variable)]
		public FsmFloat MoveSpeed;

		// Token: 0x04007DA4 RID: 32164
		[UIHint(UIHint.Variable)]
		public FsmString ClimbUpAnim;

		// Token: 0x04007DA5 RID: 32165
		[UIHint(UIHint.Variable)]
		public FsmString ClimbDownAnim;
	}
}
