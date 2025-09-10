using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001306 RID: 4870
	public class ActivateLookAnimNPC : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		// Token: 0x06007E91 RID: 32401 RVA: 0x00259594 File Offset: 0x00257794
		public bool HideFacing()
		{
			return !this.Activate.Value;
		}

		// Token: 0x06007E92 RID: 32402 RVA: 0x002595A4 File Offset: 0x002577A4
		public override void Reset()
		{
			base.Reset();
			this.Activate = null;
			this.IsFacingLeft = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06007E93 RID: 32403 RVA: 0x002595C5 File Offset: 0x002577C5
		protected override void DoAction(LookAnimNPC lookAnim)
		{
			if (!this.Activate.Value)
			{
				lookAnim.Deactivate();
				return;
			}
			if (this.IsFacingLeft.IsNone)
			{
				lookAnim.Activate();
				return;
			}
			lookAnim.Activate(this.IsFacingLeft.Value);
		}

		// Token: 0x04007E4F RID: 32335
		public FsmBool Activate;

		// Token: 0x04007E50 RID: 32336
		[HideIf("HideFacing")]
		public FsmBool IsFacingLeft;
	}
}
