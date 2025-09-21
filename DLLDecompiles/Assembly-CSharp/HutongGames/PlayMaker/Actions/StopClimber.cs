using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A1 RID: 4769
	[ActionCategory("Enemy AI")]
	public class StopClimber : FsmStateAction
	{
		// Token: 0x06007D1F RID: 32031 RVA: 0x002557F2 File Offset: 0x002539F2
		public override void Reset()
		{
			this.Target = null;
			this.WaitForTurn = true;
		}

		// Token: 0x06007D20 RID: 32032 RVA: 0x00255808 File Offset: 0x00253A08
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.climber = safe.GetComponent<Climber>();
			}
			if (this.climber == null)
			{
				base.Finish();
				return;
			}
			if (this.CanStop())
			{
				this.Stop();
			}
		}

		// Token: 0x06007D21 RID: 32033 RVA: 0x00255859 File Offset: 0x00253A59
		public override void OnUpdate()
		{
			if (this.CanStop())
			{
				this.Stop();
			}
		}

		// Token: 0x06007D22 RID: 32034 RVA: 0x00255869 File Offset: 0x00253A69
		private void Stop()
		{
			this.climber.enabled = false;
			base.Finish();
		}

		// Token: 0x06007D23 RID: 32035 RVA: 0x0025587D File Offset: 0x00253A7D
		private bool CanStop()
		{
			return !this.WaitForTurn.Value || !this.climber.IsTurning;
		}

		// Token: 0x04007D28 RID: 32040
		public FsmOwnerDefault Target;

		// Token: 0x04007D29 RID: 32041
		public FsmBool WaitForTurn;

		// Token: 0x04007D2A RID: 32042
		private Climber climber;
	}
}
