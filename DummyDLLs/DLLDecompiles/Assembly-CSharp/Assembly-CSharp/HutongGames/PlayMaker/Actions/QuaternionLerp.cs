using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001008 RID: 4104
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Interpolates between from and to by t and normalizes the result afterwards.")]
	public class QuaternionLerp : QuaternionBaseAction
	{
		// Token: 0x060070E1 RID: 28897 RVA: 0x0022C734 File Offset: 0x0022A934
		public override void Reset()
		{
			this.fromQuaternion = new FsmQuaternion
			{
				UseVariable = true
			};
			this.toQuaternion = new FsmQuaternion
			{
				UseVariable = true
			};
			this.amount = 0.5f;
			this.storeResult = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070E2 RID: 28898 RVA: 0x0022C78A File Offset: 0x0022A98A
		public override void OnEnter()
		{
			this.DoQuatLerp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070E3 RID: 28899 RVA: 0x0022C7A0 File Offset: 0x0022A9A0
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatLerp();
			}
		}

		// Token: 0x060070E4 RID: 28900 RVA: 0x0022C7B0 File Offset: 0x0022A9B0
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatLerp();
			}
		}

		// Token: 0x060070E5 RID: 28901 RVA: 0x0022C7C1 File Offset: 0x0022A9C1
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatLerp();
			}
		}

		// Token: 0x060070E6 RID: 28902 RVA: 0x0022C7D2 File Offset: 0x0022A9D2
		private void DoQuatLerp()
		{
			this.storeResult.Value = Quaternion.Lerp(this.fromQuaternion.Value, this.toQuaternion.Value, this.amount.Value);
		}

		// Token: 0x04007089 RID: 28809
		[RequiredField]
		[Tooltip("From Quaternion.")]
		public FsmQuaternion fromQuaternion;

		// Token: 0x0400708A RID: 28810
		[RequiredField]
		[Tooltip("To Quaternion.")]
		public FsmQuaternion toQuaternion;

		// Token: 0x0400708B RID: 28811
		[RequiredField]
		[Tooltip("Interpolate between fromQuaternion and toQuaternion by this amount. Value is clamped to 0-1 range. 0 = fromQuaternion; 1 = toQuaternion; 0.5 = half way between.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat amount;

		// Token: 0x0400708C RID: 28812
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in this quaternion variable.")]
		public FsmQuaternion storeResult;
	}
}
