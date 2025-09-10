using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C71 RID: 3185
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the mid point between two objects")]
	public class GetMidPoint : FsmStateAction
	{
		// Token: 0x0600601D RID: 24605 RVA: 0x001E71BA File Offset: 0x001E53BA
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.midPoint = null;
			this.everyFrame = false;
		}

		// Token: 0x0600601E RID: 24606 RVA: 0x001E71D8 File Offset: 0x001E53D8
		public override void OnEnter()
		{
			this.DoGetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600601F RID: 24607 RVA: 0x001E71EE File Offset: 0x001E53EE
		public override void OnUpdate()
		{
			this.DoGetPosition();
		}

		// Token: 0x06006020 RID: 24608 RVA: 0x001E71F8 File Offset: 0x001E53F8
		private void DoGetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.target.Value == null)
			{
				return;
			}
			Vector3 position = ownerDefaultTarget.transform.position;
			Vector3 position2 = this.target.Value.transform.position;
			Vector3 value = new Vector3(position.x + (position2.x - position.x) / 2f, position.y + (position2.y - position.y) / 2f, position.z + (position2.z - position.z) / 2f);
			this.midPoint.Value = value;
		}

		// Token: 0x04005D78 RID: 23928
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D79 RID: 23929
		public FsmGameObject target;

		// Token: 0x04005D7A RID: 23930
		[UIHint(UIHint.Variable)]
		public FsmVector3 midPoint;

		// Token: 0x04005D7B RID: 23931
		public bool everyFrame;
	}
}
