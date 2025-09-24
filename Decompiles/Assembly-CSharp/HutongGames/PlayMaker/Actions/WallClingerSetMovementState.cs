using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012CE RID: 4814
	[ActionCategory("Enemy AI")]
	public class WallClingerSetMovementState : FsmStateAction
	{
		// Token: 0x06007DB4 RID: 32180 RVA: 0x002570F8 File Offset: 0x002552F8
		public override void Reset()
		{
			this.Target = null;
			this.clinger = null;
			this.MovementState = null;
		}

		// Token: 0x06007DB5 RID: 32181 RVA: 0x00257110 File Offset: 0x00255310
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
			switch ((WallClingerSetMovementState.MovementStates)this.MovementState.Value)
			{
			case WallClingerSetMovementState.MovementStates.Inactive:
				this.clinger.IsActive = false;
				break;
			case WallClingerSetMovementState.MovementStates.Active:
				this.clinger.IsActive = true;
				break;
			case WallClingerSetMovementState.MovementStates.MovingUp:
				this.clinger.StartMovingDirection(1);
				break;
			case WallClingerSetMovementState.MovementStates.MovingDown:
				this.clinger.StartMovingDirection(-1);
				break;
			}
			base.Finish();
		}

		// Token: 0x04007D99 RID: 32153
		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007D9A RID: 32154
		private WallClinger clinger;

		// Token: 0x04007D9B RID: 32155
		[ObjectType(typeof(WallClingerSetMovementState.MovementStates))]
		public FsmEnum MovementState;

		// Token: 0x02001BEE RID: 7150
		public enum MovementStates
		{
			// Token: 0x04009F99 RID: 40857
			Inactive,
			// Token: 0x04009F9A RID: 40858
			Active,
			// Token: 0x04009F9B RID: 40859
			MovingUp,
			// Token: 0x04009F9C RID: 40860
			MovingDown
		}
	}
}
