using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001003 RID: 4099
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Creates a rotation which rotates angle degrees around axis.")]
	public class QuaternionAngleAxis : QuaternionBaseAction
	{
		// Token: 0x060070C3 RID: 28867 RVA: 0x0022C42E File Offset: 0x0022A62E
		public override void Reset()
		{
			this.angle = null;
			this.axis = null;
			this.result = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070C4 RID: 28868 RVA: 0x0022C453 File Offset: 0x0022A653
		public override void OnEnter()
		{
			this.DoQuatAngleAxis();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070C5 RID: 28869 RVA: 0x0022C469 File Offset: 0x0022A669
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatAngleAxis();
			}
		}

		// Token: 0x060070C6 RID: 28870 RVA: 0x0022C479 File Offset: 0x0022A679
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatAngleAxis();
			}
		}

		// Token: 0x060070C7 RID: 28871 RVA: 0x0022C48A File Offset: 0x0022A68A
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatAngleAxis();
			}
		}

		// Token: 0x060070C8 RID: 28872 RVA: 0x0022C49B File Offset: 0x0022A69B
		private void DoQuatAngleAxis()
		{
			this.result.Value = Quaternion.AngleAxis(this.angle.Value, this.axis.Value);
		}

		// Token: 0x0400707B RID: 28795
		[RequiredField]
		[Tooltip("The angle.")]
		public FsmFloat angle;

		// Token: 0x0400707C RID: 28796
		[RequiredField]
		[Tooltip("The rotation axis.")]
		public FsmVector3 axis;

		// Token: 0x0400707D RID: 28797
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the rotation of this quaternion variable.")]
		public FsmQuaternion result;
	}
}
