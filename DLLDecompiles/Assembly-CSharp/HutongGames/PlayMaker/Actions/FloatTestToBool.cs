using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C50 RID: 3152
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Set bools based on the comparison of 2 Floats.")]
	public class FloatTestToBool : FsmStateAction
	{
		// Token: 0x06005F89 RID: 24457 RVA: 0x001E5161 File Offset: 0x001E3361
		public override void Reset()
		{
			this.float1 = 0f;
			this.float2 = 0f;
			this.tolerance = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06005F8A RID: 24458 RVA: 0x001E519A File Offset: 0x001E339A
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F8B RID: 24459 RVA: 0x001E51B0 File Offset: 0x001E33B0
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06005F8C RID: 24460 RVA: 0x001E51B8 File Offset: 0x001E33B8
		private void DoCompare()
		{
			if (Mathf.Abs(this.float1.Value - this.float2.Value) <= this.tolerance.Value)
			{
				this.equalBool.Value = true;
			}
			else
			{
				this.equalBool.Value = false;
			}
			if (this.float1.Value < this.float2.Value)
			{
				this.lessThanBool.Value = true;
			}
			else
			{
				this.lessThanBool.Value = false;
			}
			if (this.float1.Value > this.float2.Value)
			{
				this.greaterThanBool.Value = true;
				return;
			}
			this.greaterThanBool.Value = false;
		}

		// Token: 0x04005CE0 RID: 23776
		[RequiredField]
		[Tooltip("The first float variable.")]
		public FsmFloat float1;

		// Token: 0x04005CE1 RID: 23777
		[RequiredField]
		[Tooltip("The second float variable.")]
		public FsmFloat float2;

		// Token: 0x04005CE2 RID: 23778
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		// Token: 0x04005CE3 RID: 23779
		[Tooltip("Bool set if Float 1 equals Float 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		// Token: 0x04005CE4 RID: 23780
		[Tooltip("Bool set if Float 1 is less than Float 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		// Token: 0x04005CE5 RID: 23781
		[Tooltip("Bool set if Float 1 is greater than Float 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		// Token: 0x04005CE6 RID: 23782
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
