using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200100B RID: 4107
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Rotates a rotation from towards to. This is essentially the same as Quaternion.Slerp but instead the function will ensure that the angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
	public class QuaternionRotateTowards : QuaternionBaseAction
	{
		// Token: 0x060070F6 RID: 28918 RVA: 0x0022CB14 File Offset: 0x0022AD14
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
			this.maxDegreesDelta = 10f;
			this.storeResult = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070F7 RID: 28919 RVA: 0x0022CB6A File Offset: 0x0022AD6A
		public override void OnEnter()
		{
			this.DoQuatRotateTowards();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070F8 RID: 28920 RVA: 0x0022CB80 File Offset: 0x0022AD80
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatRotateTowards();
			}
		}

		// Token: 0x060070F9 RID: 28921 RVA: 0x0022CB90 File Offset: 0x0022AD90
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatRotateTowards();
			}
		}

		// Token: 0x060070FA RID: 28922 RVA: 0x0022CBA1 File Offset: 0x0022ADA1
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatRotateTowards();
			}
		}

		// Token: 0x060070FB RID: 28923 RVA: 0x0022CBB2 File Offset: 0x0022ADB2
		private void DoQuatRotateTowards()
		{
			this.storeResult.Value = Quaternion.RotateTowards(this.fromQuaternion.Value, this.toQuaternion.Value, this.maxDegreesDelta.Value);
		}

		// Token: 0x04007093 RID: 28819
		[RequiredField]
		[Tooltip("From Quaternion.")]
		public FsmQuaternion fromQuaternion;

		// Token: 0x04007094 RID: 28820
		[RequiredField]
		[Tooltip("To Quaternion.")]
		public FsmQuaternion toQuaternion;

		// Token: 0x04007095 RID: 28821
		[RequiredField]
		[Tooltip("The angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
		public FsmFloat maxDegreesDelta;

		// Token: 0x04007096 RID: 28822
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in this quaternion variable.")]
		public FsmQuaternion storeResult;
	}
}
