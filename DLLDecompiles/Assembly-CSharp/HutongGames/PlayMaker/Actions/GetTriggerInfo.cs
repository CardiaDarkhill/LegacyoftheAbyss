using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA6 RID: 4006
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets info on the last trigger event. Typically used after a TRIGGER ENTER, TRIGGER STAY, or TRIGGER EXIT system event or a {{Trigger Event}} action. The owner of the FSM must have a trigger collider.")]
	public class GetTriggerInfo : FsmStateAction
	{
		// Token: 0x06006EAC RID: 28332 RVA: 0x00223FC3 File Offset: 0x002221C3
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.physicsMaterialName = null;
		}

		// Token: 0x06006EAD RID: 28333 RVA: 0x00223FD4 File Offset: 0x002221D4
		private void StoreTriggerInfo()
		{
			if (base.Fsm.TriggerCollider == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.TriggerCollider.gameObject;
			this.physicsMaterialName.Value = base.Fsm.TriggerCollider.material.name;
		}

		// Token: 0x06006EAE RID: 28334 RVA: 0x00224030 File Offset: 0x00222230
		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}

		// Token: 0x04006E49 RID: 28233
		[UIHint(UIHint.Variable)]
		[Tooltip("The game object that collided with the owner's trigger.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006E4A RID: 28234
		[UIHint(UIHint.Variable)]
		[Tooltip("Useful for triggering different effects. Audio, particles...")]
		public FsmString physicsMaterialName;
	}
}
