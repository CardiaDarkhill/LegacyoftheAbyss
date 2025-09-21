using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FBF RID: 4031
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last collision 2D event and store in variables. See Unity and PlayMaker docs on Unity 2D physics.")]
	public class GetCollision2dInfo : FsmStateAction
	{
		// Token: 0x06006F44 RID: 28484 RVA: 0x002262C4 File Offset: 0x002244C4
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.relativeVelocity = null;
			this.relativeSpeed = null;
			this.contactPoint = null;
			this.contactNormal = null;
			this.shapeCount = null;
			this.physics2dMaterialName = null;
		}

		// Token: 0x06006F45 RID: 28485 RVA: 0x002262F8 File Offset: 0x002244F8
		private void StoreCollisionInfo()
		{
			if (base.Fsm.Collision2DInfo == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.Collision2DInfo.gameObject;
			this.relativeSpeed.Value = base.Fsm.Collision2DInfo.relativeVelocity.magnitude;
			this.relativeVelocity.Value = base.Fsm.Collision2DInfo.relativeVelocity;
			this.physics2dMaterialName.Value = ((base.Fsm.Collision2DInfo.collider.sharedMaterial != null) ? base.Fsm.Collision2DInfo.collider.sharedMaterial.name : "");
			this.shapeCount.Value = base.Fsm.Collision2DInfo.collider.shapeCount;
			if (base.Fsm.Collision2DInfo.contacts != null && base.Fsm.Collision2DInfo.contacts.Length != 0)
			{
				this.contactPoint.Value = base.Fsm.Collision2DInfo.contacts[0].point;
				this.contactNormal.Value = base.Fsm.Collision2DInfo.contacts[0].normal;
			}
		}

		// Token: 0x06006F46 RID: 28486 RVA: 0x00226455 File Offset: 0x00224655
		public override void OnEnter()
		{
			this.StoreCollisionInfo();
			base.Finish();
		}

		// Token: 0x04006ED9 RID: 28377
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006EDA RID: 28378
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the relative velocity of the collision.")]
		public FsmVector3 relativeVelocity;

		// Token: 0x04006EDB RID: 28379
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
		public FsmFloat relativeSpeed;

		// Token: 0x04006EDC RID: 28380
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
		public FsmVector3 contactPoint;

		// Token: 0x04006EDD RID: 28381
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
		public FsmVector3 contactNormal;

		// Token: 0x04006EDE RID: 28382
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of separate shaped regions in the collider.")]
		public FsmInt shapeCount;

		// Token: 0x04006EDF RID: 28383
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the name of the physics 2D material of the colliding GameObject. Useful for triggering different effects. Audio, particles...")]
		public FsmString physics2dMaterialName;
	}
}
