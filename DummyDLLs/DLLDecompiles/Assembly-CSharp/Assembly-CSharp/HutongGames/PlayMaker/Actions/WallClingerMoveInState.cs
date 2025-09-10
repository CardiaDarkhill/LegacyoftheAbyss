using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012CD RID: 4813
	[ActionCategory("Enemy AI")]
	public class WallClingerMoveInState : FsmStateAction
	{
		// Token: 0x06007DB0 RID: 32176 RVA: 0x00257038 File Offset: 0x00255238
		public override void Reset()
		{
			this.Target = null;
			this.clinger = null;
		}

		// Token: 0x06007DB1 RID: 32177 RVA: 0x00257048 File Offset: 0x00255248
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
			switch ((WallClingerMoveInState.InitialMoveStates)this.InitialMoveState.Value)
			{
			case WallClingerMoveInState.InitialMoveStates.Random:
				this.clinger.IsActive = true;
				return;
			case WallClingerMoveInState.InitialMoveStates.Up:
				this.clinger.StartMovingDirection(1);
				return;
			case WallClingerMoveInState.InitialMoveStates.Down:
				this.clinger.StartMovingDirection(-1);
				return;
			default:
				return;
			}
		}

		// Token: 0x06007DB2 RID: 32178 RVA: 0x002570D4 File Offset: 0x002552D4
		public override void OnExit()
		{
			if (!this.clinger)
			{
				return;
			}
			this.clinger.IsActive = false;
		}

		// Token: 0x04007D96 RID: 32150
		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007D97 RID: 32151
		private WallClinger clinger;

		// Token: 0x04007D98 RID: 32152
		[ObjectType(typeof(WallClingerMoveInState.InitialMoveStates))]
		public FsmEnum InitialMoveState;

		// Token: 0x02001BED RID: 7149
		public enum InitialMoveStates
		{
			// Token: 0x04009F95 RID: 40853
			Random,
			// Token: 0x04009F96 RID: 40854
			Up,
			// Token: 0x04009F97 RID: 40855
			Down
		}
	}
}
