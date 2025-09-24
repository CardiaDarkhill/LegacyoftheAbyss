using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001009 RID: 4105
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Creates a rotation that looks along forward with the head upwards along upwards.")]
	public class QuaternionLookRotation : QuaternionBaseAction
	{
		// Token: 0x060070E8 RID: 28904 RVA: 0x0022C80D File Offset: 0x0022AA0D
		public override void Reset()
		{
			this.direction = null;
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.result = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070E9 RID: 28905 RVA: 0x0022C83D File Offset: 0x0022AA3D
		public override void OnEnter()
		{
			this.DoQuatLookRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070EA RID: 28906 RVA: 0x0022C853 File Offset: 0x0022AA53
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatLookRotation();
			}
		}

		// Token: 0x060070EB RID: 28907 RVA: 0x0022C863 File Offset: 0x0022AA63
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatLookRotation();
			}
		}

		// Token: 0x060070EC RID: 28908 RVA: 0x0022C874 File Offset: 0x0022AA74
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatLookRotation();
			}
		}

		// Token: 0x060070ED RID: 28909 RVA: 0x0022C888 File Offset: 0x0022AA88
		private void DoQuatLookRotation()
		{
			if (!this.upVector.IsNone)
			{
				this.result.Value = Quaternion.LookRotation(this.direction.Value, this.upVector.Value);
				return;
			}
			this.result.Value = Quaternion.LookRotation(this.direction.Value);
		}

		// Token: 0x0400708D RID: 28813
		[RequiredField]
		[Tooltip("the rotation direction")]
		public FsmVector3 direction;

		// Token: 0x0400708E RID: 28814
		[Tooltip("The up direction")]
		public FsmVector3 upVector;

		// Token: 0x0400708F RID: 28815
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the inverse of the rotation variable.")]
		public FsmQuaternion result;
	}
}
