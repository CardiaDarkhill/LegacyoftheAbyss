using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001002 RID: 4098
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Get the vector3 from a quaternion multiplied by a vector.")]
	public class GetQuaternionMultipliedByVector : QuaternionBaseAction
	{
		// Token: 0x060070BC RID: 28860 RVA: 0x0022C391 File Offset: 0x0022A591
		public override void Reset()
		{
			this.quaternion = null;
			this.vector3 = null;
			this.result = null;
			this.everyFrame = false;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070BD RID: 28861 RVA: 0x0022C3B6 File Offset: 0x0022A5B6
		public override void OnEnter()
		{
			this.DoQuatMult();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070BE RID: 28862 RVA: 0x0022C3CC File Offset: 0x0022A5CC
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatMult();
			}
		}

		// Token: 0x060070BF RID: 28863 RVA: 0x0022C3DC File Offset: 0x0022A5DC
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatMult();
			}
		}

		// Token: 0x060070C0 RID: 28864 RVA: 0x0022C3ED File Offset: 0x0022A5ED
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatMult();
			}
		}

		// Token: 0x060070C1 RID: 28865 RVA: 0x0022C3FE File Offset: 0x0022A5FE
		private void DoQuatMult()
		{
			this.result.Value = this.quaternion.Value * this.vector3.Value;
		}

		// Token: 0x04007078 RID: 28792
		[RequiredField]
		[Tooltip("The quaternion to multiply")]
		public FsmQuaternion quaternion;

		// Token: 0x04007079 RID: 28793
		[RequiredField]
		[Tooltip("The vector3 to multiply")]
		public FsmVector3 vector3;

		// Token: 0x0400707A RID: 28794
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting vector3")]
		public FsmVector3 result;
	}
}
