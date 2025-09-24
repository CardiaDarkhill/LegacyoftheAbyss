using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012CF RID: 4815
	[ActionCategory("Enemy AI")]
	public class WallClingerSetOptions : FsmStateAction
	{
		// Token: 0x06007DB7 RID: 32183 RVA: 0x002571BF File Offset: 0x002553BF
		public override void Reset()
		{
			this.Target = null;
			this.clinger = null;
			this.MoveSpeed = null;
			this.ClimbUpAnim = null;
			this.ClimbDownAnim = null;
		}

		// Token: 0x06007DB8 RID: 32184 RVA: 0x002571E4 File Offset: 0x002553E4
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
			if (!this.MoveSpeed.IsNone)
			{
				this.clinger.MoveSpeed = this.MoveSpeed.Value;
			}
			if (!this.ClimbUpAnim.IsNone)
			{
				this.clinger.ClimbUpAnim = this.ClimbUpAnim.Value;
			}
			if (!this.ClimbDownAnim.IsNone)
			{
				this.clinger.ClimbDownAnim = this.ClimbDownAnim.Value;
			}
			base.Finish();
		}

		// Token: 0x04007D9C RID: 32156
		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007D9D RID: 32157
		private WallClinger clinger;

		// Token: 0x04007D9E RID: 32158
		public FsmFloat MoveSpeed;

		// Token: 0x04007D9F RID: 32159
		public FsmString ClimbUpAnim;

		// Token: 0x04007DA0 RID: 32160
		public FsmString ClimbDownAnim;
	}
}
