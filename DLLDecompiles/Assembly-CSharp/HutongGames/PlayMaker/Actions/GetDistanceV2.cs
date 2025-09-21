using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C68 RID: 3176
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
	public class GetDistanceV2 : FsmStateAction
	{
		// Token: 0x06005FF7 RID: 24567 RVA: 0x001E6528 File Offset: 0x001E4728
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06005FF8 RID: 24568 RVA: 0x001E6546 File Offset: 0x001E4746
		public override void OnEnter()
		{
			this.DoGetDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FF9 RID: 24569 RVA: 0x001E655C File Offset: 0x001E475C
		public override void OnUpdate()
		{
			this.DoGetDistance();
		}

		// Token: 0x06005FFA RID: 24570 RVA: 0x001E6564 File Offset: 0x001E4764
		private void DoGetDistance()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.target.Value == null || this.storeResult == null)
			{
				return;
			}
			Vector3 b = new Vector3(this.target.Value.transform.position.x + this.targetOffsetX.Value, this.target.Value.transform.position.y + this.targetOffsetY.Value, this.target.Value.transform.position.z);
			this.storeResult.Value = Vector3.Distance(ownerDefaultTarget.transform.position, b);
		}

		// Token: 0x04005D4A RID: 23882
		[RequiredField]
		[Tooltip("Measure distance from this GameObject.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D4B RID: 23883
		[RequiredField]
		[Tooltip("Target GameObject.")]
		public FsmGameObject target;

		// Token: 0x04005D4C RID: 23884
		public FsmFloat targetOffsetX;

		// Token: 0x04005D4D RID: 23885
		public FsmFloat targetOffsetY;

		// Token: 0x04005D4E RID: 23886
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x04005D4F RID: 23887
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
