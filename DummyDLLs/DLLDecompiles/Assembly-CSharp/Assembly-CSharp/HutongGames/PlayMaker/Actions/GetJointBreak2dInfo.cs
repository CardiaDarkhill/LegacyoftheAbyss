using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC1 RID: 4033
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last joint break 2D event.")]
	public class GetJointBreak2dInfo : FsmStateAction
	{
		// Token: 0x06006F4D RID: 28493 RVA: 0x002264E7 File Offset: 0x002246E7
		public override void Reset()
		{
			this.brokenJoint = null;
			this.reactionForce = null;
			this.reactionTorque = null;
		}

		// Token: 0x06006F4E RID: 28494 RVA: 0x00226500 File Offset: 0x00224700
		private void StoreInfo()
		{
			if (base.Fsm.BrokenJoint2D == null)
			{
				return;
			}
			this.brokenJoint.Value = base.Fsm.BrokenJoint2D;
			this.reactionForce.Value = base.Fsm.BrokenJoint2D.reactionForce;
			this.reactionForceMagnitude.Value = base.Fsm.BrokenJoint2D.reactionForce.magnitude;
			this.reactionTorque.Value = base.Fsm.BrokenJoint2D.reactionTorque;
		}

		// Token: 0x06006F4F RID: 28495 RVA: 0x00226590 File Offset: 0x00224790
		public override void OnEnter()
		{
			this.StoreInfo();
			base.Finish();
		}

		// Token: 0x04006EE3 RID: 28387
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Joint2D))]
		[Tooltip("Get the broken joint.")]
		public FsmObject brokenJoint;

		// Token: 0x04006EE4 RID: 28388
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the reaction force exerted by the broken joint. Unity 5.3+")]
		public FsmVector2 reactionForce;

		// Token: 0x04006EE5 RID: 28389
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the magnitude of the reaction force exerted by the broken joint. Unity 5.3+")]
		public FsmFloat reactionForceMagnitude;

		// Token: 0x04006EE6 RID: 28390
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the reaction torque exerted by the broken joint. Unity 5.3+")]
		public FsmFloat reactionTorque;
	}
}
