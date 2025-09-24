using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FFF RID: 4095
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Gets a quaternion as euler angles.")]
	public class GetQuaternionEulerAngles : QuaternionBaseAction
	{
		// Token: 0x060070A7 RID: 28839 RVA: 0x0022C1BB File Offset: 0x0022A3BB
		public override void Reset()
		{
			this.quaternion = null;
			this.eulerAngles = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070A8 RID: 28840 RVA: 0x0022C1D9 File Offset: 0x0022A3D9
		public override void OnEnter()
		{
			this.GetQuatEuler();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070A9 RID: 28841 RVA: 0x0022C1EF File Offset: 0x0022A3EF
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.GetQuatEuler();
			}
		}

		// Token: 0x060070AA RID: 28842 RVA: 0x0022C1FF File Offset: 0x0022A3FF
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.GetQuatEuler();
			}
		}

		// Token: 0x060070AB RID: 28843 RVA: 0x0022C210 File Offset: 0x0022A410
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.GetQuatEuler();
			}
		}

		// Token: 0x060070AC RID: 28844 RVA: 0x0022C224 File Offset: 0x0022A424
		private void GetQuatEuler()
		{
			this.eulerAngles.Value = this.quaternion.Value.eulerAngles;
		}

		// Token: 0x04007070 RID: 28784
		[RequiredField]
		[Tooltip("The rotation")]
		public FsmQuaternion quaternion;

		// Token: 0x04007071 RID: 28785
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The euler angles of the quaternion.")]
		public FsmVector3 eulerAngles;
	}
}
