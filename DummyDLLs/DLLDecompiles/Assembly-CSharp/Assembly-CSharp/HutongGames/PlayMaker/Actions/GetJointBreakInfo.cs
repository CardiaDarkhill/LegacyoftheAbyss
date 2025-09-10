using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA0 RID: 4000
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets info on the last joint break event.")]
	public class GetJointBreakInfo : FsmStateAction
	{
		// Token: 0x06006E92 RID: 28306 RVA: 0x00223C1C File Offset: 0x00221E1C
		public override void Reset()
		{
			this.breakForce = null;
		}

		// Token: 0x06006E93 RID: 28307 RVA: 0x00223C25 File Offset: 0x00221E25
		public override void OnEnter()
		{
			this.breakForce.Value = base.Fsm.JointBreakForce;
			base.Finish();
		}

		// Token: 0x04006E38 RID: 28216
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the force that broke the joint.")]
		public FsmFloat breakForce;
	}
}
