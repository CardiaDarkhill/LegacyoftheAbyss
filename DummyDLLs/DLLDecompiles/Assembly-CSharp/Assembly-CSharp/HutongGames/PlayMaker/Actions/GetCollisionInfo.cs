using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F9F RID: 3999
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets info on the last collision event. Typically used after a COLLISION ENTER, COLLISION STAY, or COLLISION EXIT system event or Collision Event action to get more info on the collision.")]
	public class GetCollisionInfo : FsmStateAction
	{
		// Token: 0x06006E8E RID: 28302 RVA: 0x00223ACE File Offset: 0x00221CCE
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.relativeVelocity = null;
			this.relativeSpeed = null;
			this.contactPoint = null;
			this.contactNormal = null;
			this.physicsMaterialName = null;
		}

		// Token: 0x06006E8F RID: 28303 RVA: 0x00223AFC File Offset: 0x00221CFC
		private void StoreCollisionInfo()
		{
			if (base.Fsm.CollisionInfo == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.CollisionInfo.gameObject;
			this.relativeSpeed.Value = base.Fsm.CollisionInfo.relativeVelocity.magnitude;
			this.relativeVelocity.Value = base.Fsm.CollisionInfo.relativeVelocity;
			this.physicsMaterialName.Value = base.Fsm.CollisionInfo.collider.material.name;
			if (base.Fsm.CollisionInfo.contacts != null && base.Fsm.CollisionInfo.contacts.Length != 0)
			{
				this.contactPoint.Value = base.Fsm.CollisionInfo.contacts[0].point;
				this.contactNormal.Value = base.Fsm.CollisionInfo.contacts[0].normal;
			}
		}

		// Token: 0x06006E90 RID: 28304 RVA: 0x00223C06 File Offset: 0x00221E06
		public override void OnEnter()
		{
			this.StoreCollisionInfo();
			base.Finish();
		}

		// Token: 0x04006E32 RID: 28210
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006E33 RID: 28211
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the relative velocity of the collision.")]
		public FsmVector3 relativeVelocity;

		// Token: 0x04006E34 RID: 28212
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
		public FsmFloat relativeSpeed;

		// Token: 0x04006E35 RID: 28213
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
		public FsmVector3 contactPoint;

		// Token: 0x04006E36 RID: 28214
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
		public FsmVector3 contactNormal;

		// Token: 0x04006E37 RID: 28215
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the name of the physics material on the Game Object Hit. Useful for triggering different effects. Audio, particles...")]
		public FsmString physicsMaterialName;
	}
}
