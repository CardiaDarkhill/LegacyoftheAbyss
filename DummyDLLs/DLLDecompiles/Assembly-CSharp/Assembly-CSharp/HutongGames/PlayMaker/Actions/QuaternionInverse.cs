using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001007 RID: 4103
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Inverse a quaternion")]
	public class QuaternionInverse : QuaternionBaseAction
	{
		// Token: 0x060070DA RID: 28890 RVA: 0x0022C6A6 File Offset: 0x0022A8A6
		public override void Reset()
		{
			this.rotation = null;
			this.result = null;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070DB RID: 28891 RVA: 0x0022C6C4 File Offset: 0x0022A8C4
		public override void OnEnter()
		{
			this.DoQuatInverse();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070DC RID: 28892 RVA: 0x0022C6DA File Offset: 0x0022A8DA
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatInverse();
			}
		}

		// Token: 0x060070DD RID: 28893 RVA: 0x0022C6EA File Offset: 0x0022A8EA
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatInverse();
			}
		}

		// Token: 0x060070DE RID: 28894 RVA: 0x0022C6FB File Offset: 0x0022A8FB
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatInverse();
			}
		}

		// Token: 0x060070DF RID: 28895 RVA: 0x0022C70C File Offset: 0x0022A90C
		private void DoQuatInverse()
		{
			this.result.Value = Quaternion.Inverse(this.rotation.Value);
		}

		// Token: 0x04007087 RID: 28807
		[RequiredField]
		[Tooltip("the rotation")]
		public FsmQuaternion rotation;

		// Token: 0x04007088 RID: 28808
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the inverse of the rotation variable.")]
		public FsmQuaternion result;
	}
}
