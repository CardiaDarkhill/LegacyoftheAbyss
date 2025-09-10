using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001000 RID: 4096
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Creates a rotation which rotates from fromDirection to toDirection. Usually you use this to rotate a transform so that one of its axes, e.g., the y-axis - follows a target direction toDirection in world space.")]
	public class GetQuaternionFromRotation : QuaternionBaseAction
	{
		// Token: 0x060070AE RID: 28846 RVA: 0x0022C257 File Offset: 0x0022A457
		public override void Reset()
		{
			this.fromDirection = null;
			this.toDirection = null;
			this.result = null;
			this.everyFrame = false;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070AF RID: 28847 RVA: 0x0022C27C File Offset: 0x0022A47C
		public override void OnEnter()
		{
			this.DoQuatFromRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070B0 RID: 28848 RVA: 0x0022C292 File Offset: 0x0022A492
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatFromRotation();
			}
		}

		// Token: 0x060070B1 RID: 28849 RVA: 0x0022C2A2 File Offset: 0x0022A4A2
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatFromRotation();
			}
		}

		// Token: 0x060070B2 RID: 28850 RVA: 0x0022C2B3 File Offset: 0x0022A4B3
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatFromRotation();
			}
		}

		// Token: 0x060070B3 RID: 28851 RVA: 0x0022C2C4 File Offset: 0x0022A4C4
		private void DoQuatFromRotation()
		{
			this.result.Value = Quaternion.FromToRotation(this.fromDirection.Value, this.toDirection.Value);
		}

		// Token: 0x04007072 RID: 28786
		[RequiredField]
		[Tooltip("the 'from' direction")]
		public FsmVector3 fromDirection;

		// Token: 0x04007073 RID: 28787
		[RequiredField]
		[Tooltip("the 'to' direction")]
		public FsmVector3 toDirection;

		// Token: 0x04007074 RID: 28788
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("the resulting quaternion")]
		public FsmQuaternion result;
	}
}
