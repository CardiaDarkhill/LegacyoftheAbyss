using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001001 RID: 4097
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Get the quaternion from a quaternion multiplied by a quaternion.")]
	public class GetQuaternionMultipliedByQuaternion : QuaternionBaseAction
	{
		// Token: 0x060070B5 RID: 28853 RVA: 0x0022C2F4 File Offset: 0x0022A4F4
		public override void Reset()
		{
			this.quaternionA = null;
			this.quaternionB = null;
			this.result = null;
			this.everyFrame = false;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070B6 RID: 28854 RVA: 0x0022C319 File Offset: 0x0022A519
		public override void OnEnter()
		{
			this.DoQuatMult();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070B7 RID: 28855 RVA: 0x0022C32F File Offset: 0x0022A52F
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatMult();
			}
		}

		// Token: 0x060070B8 RID: 28856 RVA: 0x0022C33F File Offset: 0x0022A53F
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatMult();
			}
		}

		// Token: 0x060070B9 RID: 28857 RVA: 0x0022C350 File Offset: 0x0022A550
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatMult();
			}
		}

		// Token: 0x060070BA RID: 28858 RVA: 0x0022C361 File Offset: 0x0022A561
		private void DoQuatMult()
		{
			this.result.Value = this.quaternionA.Value * this.quaternionB.Value;
		}

		// Token: 0x04007075 RID: 28789
		[RequiredField]
		[Tooltip("The first quaternion to multiply")]
		public FsmQuaternion quaternionA;

		// Token: 0x04007076 RID: 28790
		[RequiredField]
		[Tooltip("The second quaternion to multiply")]
		public FsmQuaternion quaternionB;

		// Token: 0x04007077 RID: 28791
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting quaternion")]
		public FsmQuaternion result;
	}
}
