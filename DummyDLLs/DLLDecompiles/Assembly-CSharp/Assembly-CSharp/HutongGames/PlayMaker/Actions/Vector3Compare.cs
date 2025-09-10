using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDD RID: 3293
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Vector3's.")]
	public class Vector3Compare : FsmStateAction
	{
		// Token: 0x06006202 RID: 25090 RVA: 0x001EFB83 File Offset: 0x001EDD83
		public override void Reset()
		{
			this.vector3Variable1 = null;
			this.vector3Variable2 = null;
			this.tolerance = 0f;
			this.equal = null;
			this.notEqual = null;
			this.everyFrame = false;
		}

		// Token: 0x06006203 RID: 25091 RVA: 0x001EFBB8 File Offset: 0x001EDDB8
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006204 RID: 25092 RVA: 0x001EFBCE File Offset: 0x001EDDCE
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06006205 RID: 25093 RVA: 0x001EFBD8 File Offset: 0x001EDDD8
		private void DoCompare()
		{
			if (this.vector3Variable1 == null || this.vector3Variable2 == null)
			{
				return;
			}
			if (Mathf.Abs(this.vector3Variable1.Value.x - this.vector3Variable2.Value.x) <= this.tolerance.Value && Mathf.Abs(this.vector3Variable1.Value.y - this.vector3Variable2.Value.y) <= this.tolerance.Value && Mathf.Abs(this.vector3Variable1.Value.z - this.vector3Variable2.Value.z) <= this.tolerance.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			base.Fsm.Event(this.notEqual);
		}

		// Token: 0x06006206 RID: 25094 RVA: 0x001EFCB2 File Offset: 0x001EDEB2
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.notEqual))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006014 RID: 24596
		[RequiredField]
		public FsmVector3 vector3Variable1;

		// Token: 0x04006015 RID: 24597
		[RequiredField]
		public FsmVector3 vector3Variable2;

		// Token: 0x04006016 RID: 24598
		[RequiredField]
		public FsmFloat tolerance;

		// Token: 0x04006017 RID: 24599
		[Tooltip("Event sent if Vector3 1 equals Vector3 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x04006018 RID: 24600
		[Tooltip("Event sent if the Vector3's are not equal")]
		public FsmEvent notEqual;

		// Token: 0x04006019 RID: 24601
		public bool everyFrame;
	}
}
