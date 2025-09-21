using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FCA RID: 4042
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last Trigger 2d event and store in variables.  See Unity and PlayMaker docs on Unity 2D physics.")]
	public class GetTrigger2dInfo : FsmStateAction
	{
		// Token: 0x06006F78 RID: 28536 RVA: 0x00227811 File Offset: 0x00225A11
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.shapeCount = null;
			this.physics2dMaterialName = null;
		}

		// Token: 0x06006F79 RID: 28537 RVA: 0x00227828 File Offset: 0x00225A28
		private void StoreTriggerInfo()
		{
			if (base.Fsm.TriggerCollider2D == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.TriggerCollider2D.gameObject;
			this.shapeCount.Value = base.Fsm.TriggerCollider2D.shapeCount;
			this.physics2dMaterialName.Value = ((base.Fsm.TriggerCollider2D.sharedMaterial != null) ? base.Fsm.TriggerCollider2D.sharedMaterial.name : "");
		}

		// Token: 0x06006F7A RID: 28538 RVA: 0x002278BE File Offset: 0x00225ABE
		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}

		// Token: 0x04006F46 RID: 28486
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006F47 RID: 28487
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of separate shaped regions in the collider.")]
		public FsmInt shapeCount;

		// Token: 0x04006F48 RID: 28488
		[UIHint(UIHint.Variable)]
		[Tooltip("Useful for triggering different effects. Audio, particles...")]
		public FsmString physics2dMaterialName;
	}
}
