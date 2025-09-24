using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200130A RID: 4874
	public sealed class LookAnimNpcForceFacing : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		// Token: 0x06007E9E RID: 32414 RVA: 0x002596A0 File Offset: 0x002578A0
		public bool HideFacing()
		{
			return !this.ForceFacing.Value;
		}

		// Token: 0x06007E9F RID: 32415 RVA: 0x002596B0 File Offset: 0x002578B0
		public override void Reset()
		{
			base.Reset();
			this.ForceFacing = null;
			this.FaceLeft = null;
			this.WaitUntilFinished = null;
			this.FinishedTurning = null;
		}

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x06007EA0 RID: 32416 RVA: 0x002596D4 File Offset: 0x002578D4
		protected override bool AutoFinish
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06007EA1 RID: 32417 RVA: 0x002596D8 File Offset: 0x002578D8
		protected override void DoAction(LookAnimNPC lookAnim)
		{
			this.hasLookNpc = (lookAnim != null);
			this.lookAnimNpc = lookAnim;
			if (this.ForceFacing.Value)
			{
				lookAnim.ForceTurn(this.FaceLeft.Value);
			}
			else
			{
				lookAnim.UnlockTurn();
			}
			this.notTurningCount = 0;
			if (!this.WaitUntilFinished.Value)
			{
				base.Finish();
				return;
			}
		}

		// Token: 0x06007EA2 RID: 32418 RVA: 0x0025973C File Offset: 0x0025793C
		protected override void DoActionNoComponent(GameObject target)
		{
			this.FinishTurn();
		}

		// Token: 0x06007EA3 RID: 32419 RVA: 0x00259744 File Offset: 0x00257944
		public override void OnUpdate()
		{
			if (!this.hasLookNpc)
			{
				this.FinishTurn();
				return;
			}
			if (!this.lookAnimNpc.IsTurning)
			{
				this.notTurningCount++;
			}
			else
			{
				this.notTurningCount = 0;
			}
			if (this.notTurningCount > 1)
			{
				this.FinishTurn();
			}
		}

		// Token: 0x06007EA4 RID: 32420 RVA: 0x00259793 File Offset: 0x00257993
		private void FinishTurn()
		{
			base.Fsm.Event(this.FinishedTurning);
			base.Finish();
		}

		// Token: 0x04007E53 RID: 32339
		public FsmBool ForceFacing;

		// Token: 0x04007E54 RID: 32340
		[HideIf("HideFacing")]
		public FsmBool FaceLeft;

		// Token: 0x04007E55 RID: 32341
		public FsmBool WaitUntilFinished;

		// Token: 0x04007E56 RID: 32342
		public FsmEvent FinishedTurning;

		// Token: 0x04007E57 RID: 32343
		private int notTurningCount;

		// Token: 0x04007E58 RID: 32344
		private LookAnimNPC lookAnimNpc;

		// Token: 0x04007E59 RID: 32345
		private bool hasLookNpc;
	}
}
