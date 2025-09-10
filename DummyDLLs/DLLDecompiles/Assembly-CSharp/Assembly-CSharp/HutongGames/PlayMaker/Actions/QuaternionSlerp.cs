using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200100C RID: 4108
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Spherically interpolates between from and to by t.")]
	public class QuaternionSlerp : QuaternionBaseAction
	{
		// Token: 0x060070FD RID: 28925 RVA: 0x0022CBF0 File Offset: 0x0022ADF0
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
			this.amount = 0.1f;
			this.storeResult = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070FE RID: 28926 RVA: 0x0022CC46 File Offset: 0x0022AE46
		public override void OnEnter()
		{
			this.DoQuatSlerp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070FF RID: 28927 RVA: 0x0022CC5C File Offset: 0x0022AE5C
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatSlerp();
			}
		}

		// Token: 0x06007100 RID: 28928 RVA: 0x0022CC6C File Offset: 0x0022AE6C
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatSlerp();
			}
		}

		// Token: 0x06007101 RID: 28929 RVA: 0x0022CC7D File Offset: 0x0022AE7D
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatSlerp();
			}
		}

		// Token: 0x06007102 RID: 28930 RVA: 0x0022CC8E File Offset: 0x0022AE8E
		private void DoQuatSlerp()
		{
			this.storeResult.Value = Quaternion.Slerp(this.fromQuaternion.Value, this.toQuaternion.Value, this.amount.Value);
		}

		// Token: 0x04007097 RID: 28823
		[RequiredField]
		[Tooltip("From Quaternion.")]
		public FsmQuaternion fromQuaternion;

		// Token: 0x04007098 RID: 28824
		[RequiredField]
		[Tooltip("To Quaternion.")]
		public FsmQuaternion toQuaternion;

		// Token: 0x04007099 RID: 28825
		[RequiredField]
		[Tooltip("Interpolate between fromQuaternion and toQuaternion by this amount. Value is clamped to 0-1 range. 0 = fromQuaternion; 1 = toQuaternion; 0.5 = half way between.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat amount;

		// Token: 0x0400709A RID: 28826
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in this quaternion variable.")]
		public FsmQuaternion storeResult;
	}
}
