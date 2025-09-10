using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001005 RID: 4101
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Check if two quaternions are equals or not. Takes in account inversed representations of quaternions")]
	public class QuaternionCompare : QuaternionBaseAction
	{
		// Token: 0x060070CC RID: 28876 RVA: 0x0022C514 File Offset: 0x0022A714
		public override void Reset()
		{
			this.Quaternion1 = new FsmQuaternion
			{
				UseVariable = true
			};
			this.Quaternion2 = new FsmQuaternion
			{
				UseVariable = true
			};
			this.equal = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070CD RID: 28877 RVA: 0x0022C561 File Offset: 0x0022A761
		public override void OnEnter()
		{
			this.DoQuatCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070CE RID: 28878 RVA: 0x0022C577 File Offset: 0x0022A777
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatCompare();
			}
		}

		// Token: 0x060070CF RID: 28879 RVA: 0x0022C587 File Offset: 0x0022A787
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatCompare();
			}
		}

		// Token: 0x060070D0 RID: 28880 RVA: 0x0022C598 File Offset: 0x0022A798
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatCompare();
			}
		}

		// Token: 0x060070D1 RID: 28881 RVA: 0x0022C5AC File Offset: 0x0022A7AC
		private void DoQuatCompare()
		{
			bool flag = Mathf.Abs(Quaternion.Dot(this.Quaternion1.Value, this.Quaternion2.Value)) > 0.999999f;
			this.equal.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.equalEvent);
				return;
			}
			base.Fsm.Event(this.notEqualEvent);
		}

		// Token: 0x04007080 RID: 28800
		[RequiredField]
		[Tooltip("First Quaternion")]
		public FsmQuaternion Quaternion1;

		// Token: 0x04007081 RID: 28801
		[RequiredField]
		[Tooltip("Second Quaternion")]
		public FsmQuaternion Quaternion2;

		// Token: 0x04007082 RID: 28802
		[Tooltip("true if Quaternions are equal")]
		public FsmBool equal;

		// Token: 0x04007083 RID: 28803
		[Tooltip("Event sent if Quaternions are equal")]
		public FsmEvent equalEvent;

		// Token: 0x04007084 RID: 28804
		[Tooltip("Event sent if Quaternions are not equal")]
		public FsmEvent notEqualEvent;
	}
}
