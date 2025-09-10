using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001006 RID: 4102
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).")]
	public class QuaternionEuler : QuaternionBaseAction
	{
		// Token: 0x060070D3 RID: 28883 RVA: 0x0022C61B File Offset: 0x0022A81B
		public override void Reset()
		{
			this.eulerAngles = null;
			this.result = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070D4 RID: 28884 RVA: 0x0022C639 File Offset: 0x0022A839
		public override void OnEnter()
		{
			this.DoQuatEuler();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070D5 RID: 28885 RVA: 0x0022C64F File Offset: 0x0022A84F
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatEuler();
			}
		}

		// Token: 0x060070D6 RID: 28886 RVA: 0x0022C65F File Offset: 0x0022A85F
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatEuler();
			}
		}

		// Token: 0x060070D7 RID: 28887 RVA: 0x0022C670 File Offset: 0x0022A870
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatEuler();
			}
		}

		// Token: 0x060070D8 RID: 28888 RVA: 0x0022C681 File Offset: 0x0022A881
		private void DoQuatEuler()
		{
			this.result.Value = Quaternion.Euler(this.eulerAngles.Value);
		}

		// Token: 0x04007085 RID: 28805
		[RequiredField]
		[Tooltip("The Euler angles.")]
		public FsmVector3 eulerAngles;

		// Token: 0x04007086 RID: 28806
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the euler angles of this quaternion variable.")]
		public FsmQuaternion result;
	}
}
