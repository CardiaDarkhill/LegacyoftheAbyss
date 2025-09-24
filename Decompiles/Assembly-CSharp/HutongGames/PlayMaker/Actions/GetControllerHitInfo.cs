using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E5C RID: 3676
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Gets info on the last Character Controller collision event. The owner of the FSM must have a character controller. Typically this action is used after a CONTROLLER COLLIDER HIT system event.")]
	public class GetControllerHitInfo : FsmStateAction
	{
		// Token: 0x060068F7 RID: 26871 RVA: 0x0020F637 File Offset: 0x0020D837
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.contactPoint = null;
			this.contactNormal = null;
			this.moveDirection = null;
			this.moveLength = null;
			this.physicsMaterialName = null;
		}

		// Token: 0x060068F8 RID: 26872 RVA: 0x0020F663 File Offset: 0x0020D863
		public override void OnPreprocess()
		{
			base.Fsm.HandleControllerColliderHit = true;
		}

		// Token: 0x060068F9 RID: 26873 RVA: 0x0020F674 File Offset: 0x0020D874
		private void StoreTriggerInfo()
		{
			if (base.Fsm.ControllerCollider == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.ControllerCollider.gameObject;
			this.contactPoint.Value = base.Fsm.ControllerCollider.point;
			this.contactNormal.Value = base.Fsm.ControllerCollider.normal;
			this.moveDirection.Value = base.Fsm.ControllerCollider.moveDirection;
			this.moveLength.Value = base.Fsm.ControllerCollider.moveLength;
			this.physicsMaterialName.Value = base.Fsm.ControllerCollider.collider.material.name;
		}

		// Token: 0x060068FA RID: 26874 RVA: 0x0020F73B File Offset: 0x0020D93B
		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}

		// Token: 0x060068FB RID: 26875 RVA: 0x0020F749 File Offset: 0x0020D949
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysicsSetup(base.Owner);
		}

		// Token: 0x04006846 RID: 26694
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject hit in the last collision.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006847 RID: 26695
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the contact point of the last collision in world coordinates.")]
		public FsmVector3 contactPoint;

		// Token: 0x04006848 RID: 26696
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the normal of the last collision.")]
		public FsmVector3 contactNormal;

		// Token: 0x04006849 RID: 26697
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the direction of the last move before the collision.")]
		public FsmVector3 moveDirection;

		// Token: 0x0400684A RID: 26698
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance of the last move before the collision.")]
		public FsmFloat moveLength;

		// Token: 0x0400684B RID: 26699
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the physics material of the Game Object Hit. Useful for triggering different effects. Audio, particles...")]
		public FsmString physicsMaterialName;
	}
}
