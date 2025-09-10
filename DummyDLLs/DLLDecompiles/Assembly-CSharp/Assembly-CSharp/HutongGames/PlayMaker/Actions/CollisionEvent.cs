using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F9C RID: 3996
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sends the specified event when the owner collides with a tagged object. Optionally store the collider and collision force in variables for later use. NOTE: Collisions are processed after other actions, so this action should be ordered last in the action list.")]
	public class CollisionEvent : FsmStateAction
	{
		// Token: 0x06006E5E RID: 28254 RVA: 0x00222FB4 File Offset: 0x002211B4
		public override void Reset()
		{
			this.gameObject = null;
			this.collision = CollisionType.OnCollisionEnter;
			this.collideTag = "";
			this.sendEvent = null;
			this.storeCollider = null;
			this.storeForce = null;
		}

		// Token: 0x06006E5F RID: 28255 RVA: 0x00222FEC File Offset: 0x002211EC
		public override void OnPreprocess()
		{
			if (this.gameObject == null)
			{
				this.gameObject = new FsmOwnerDefault();
			}
			if (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner)
			{
				this.GetProxyComponent();
				return;
			}
			switch (this.collision)
			{
			case CollisionType.OnCollisionEnter:
				base.Fsm.HandleCollisionEnter = true;
				return;
			case CollisionType.OnCollisionStay:
				base.Fsm.HandleCollisionStay = true;
				return;
			case CollisionType.OnCollisionExit:
				base.Fsm.HandleCollisionExit = true;
				return;
			case CollisionType.OnControllerColliderHit:
				base.Fsm.HandleControllerColliderHit = true;
				return;
			case CollisionType.OnParticleCollision:
				base.Fsm.HandleParticleCollision = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x06006E60 RID: 28256 RVA: 0x00223084 File Offset: 0x00221284
		public override void OnEnter()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return;
			}
			if (this.cachedProxy == null)
			{
				this.GetProxyComponent();
			}
			this.AddCallback();
			this.gameObject.GameObject.OnChange += this.UpdateCallback;
		}

		// Token: 0x06006E61 RID: 28257 RVA: 0x002230D5 File Offset: 0x002212D5
		public override void OnExit()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return;
			}
			this.RemoveCallback();
			this.gameObject.GameObject.OnChange -= this.UpdateCallback;
		}

		// Token: 0x06006E62 RID: 28258 RVA: 0x00223107 File Offset: 0x00221307
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x06006E63 RID: 28259 RVA: 0x0022311C File Offset: 0x0022131C
		private void GetProxyComponent()
		{
			this.cachedProxy = null;
			GameObject value = this.gameObject.GameObject.Value;
			if (value == null)
			{
				return;
			}
			switch (this.collision)
			{
			case CollisionType.OnCollisionEnter:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionEnter>(value);
				return;
			case CollisionType.OnCollisionStay:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionStay>(value);
				return;
			case CollisionType.OnCollisionExit:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionExit>(value);
				return;
			case CollisionType.OnControllerColliderHit:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerControllerColliderHit>(value);
				return;
			case CollisionType.OnParticleCollision:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerParticleCollision>(value);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006E64 RID: 28260 RVA: 0x002231B0 File Offset: 0x002213B0
		private void AddCallback()
		{
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.collision)
			{
			case CollisionType.OnCollisionEnter:
				this.cachedProxy.AddCollisionEventCallback(new PlayMakerProxyBase.CollisionEvent(this.CollisionEnter));
				return;
			case CollisionType.OnCollisionStay:
				this.cachedProxy.AddCollisionEventCallback(new PlayMakerProxyBase.CollisionEvent(this.CollisionStay));
				return;
			case CollisionType.OnCollisionExit:
				this.cachedProxy.AddCollisionEventCallback(new PlayMakerProxyBase.CollisionEvent(this.CollisionExit));
				return;
			case CollisionType.OnControllerColliderHit:
				this.cachedProxy.AddControllerCollisionEventCallback(new PlayMakerProxyBase.ControllerCollisionEvent(this.ControllerColliderHit));
				return;
			case CollisionType.OnParticleCollision:
				this.cachedProxy.AddParticleCollisionEventCallback(new PlayMakerProxyBase.ParticleCollisionEvent(this.ParticleCollision));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006E65 RID: 28261 RVA: 0x00223268 File Offset: 0x00221468
		private void RemoveCallback()
		{
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.collision)
			{
			case CollisionType.OnCollisionEnter:
				this.cachedProxy.RemoveCollisionEventCallback(new PlayMakerProxyBase.CollisionEvent(this.CollisionEnter));
				return;
			case CollisionType.OnCollisionStay:
				this.cachedProxy.RemoveCollisionEventCallback(new PlayMakerProxyBase.CollisionEvent(this.CollisionStay));
				return;
			case CollisionType.OnCollisionExit:
				this.cachedProxy.RemoveCollisionEventCallback(new PlayMakerProxyBase.CollisionEvent(this.CollisionExit));
				return;
			case CollisionType.OnControllerColliderHit:
				this.cachedProxy.RemoveControllerCollisionEventCallback(new PlayMakerProxyBase.ControllerCollisionEvent(this.ControllerColliderHit));
				return;
			case CollisionType.OnParticleCollision:
				this.cachedProxy.RemoveParticleCollisionEventCallback(new PlayMakerProxyBase.ParticleCollisionEvent(this.ParticleCollision));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006E66 RID: 28262 RVA: 0x00223320 File Offset: 0x00221520
		private void StoreCollisionInfo(Collision collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
			this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		// Token: 0x06006E67 RID: 28263 RVA: 0x00223357 File Offset: 0x00221557
		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.CollisionEnter(collisionInfo);
			}
		}

		// Token: 0x06006E68 RID: 28264 RVA: 0x0022336D File Offset: 0x0022156D
		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.CollisionStay(collisionInfo);
			}
		}

		// Token: 0x06006E69 RID: 28265 RVA: 0x00223383 File Offset: 0x00221583
		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.CollisionExit(collisionInfo);
			}
		}

		// Token: 0x06006E6A RID: 28266 RVA: 0x00223399 File Offset: 0x00221599
		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.ControllerColliderHit(collisionInfo);
			}
		}

		// Token: 0x06006E6B RID: 28267 RVA: 0x002233AF File Offset: 0x002215AF
		public override void DoParticleCollision(GameObject other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.ParticleCollision(other);
			}
		}

		// Token: 0x06006E6C RID: 28268 RVA: 0x002233C5 File Offset: 0x002215C5
		private void CollisionEnter(Collision collisionInfo)
		{
			if (this.collision == CollisionType.OnCollisionEnter && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006E6D RID: 28269 RVA: 0x002233F5 File Offset: 0x002215F5
		private void CollisionStay(Collision collisionInfo)
		{
			if (this.collision == CollisionType.OnCollisionStay && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006E6E RID: 28270 RVA: 0x00223426 File Offset: 0x00221626
		private void CollisionExit(Collision collisionInfo)
		{
			if (this.collision == CollisionType.OnCollisionExit && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006E6F RID: 28271 RVA: 0x00223458 File Offset: 0x00221658
		private void ControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (this.collision == CollisionType.OnControllerColliderHit && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				if (this.storeCollider != null)
				{
					this.storeCollider.Value = collisionInfo.gameObject;
				}
				this.storeForce.Value = 0f;
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006E70 RID: 28272 RVA: 0x002234B8 File Offset: 0x002216B8
		private void ParticleCollision(GameObject other)
		{
			if (this.collision == CollisionType.OnParticleCollision && FsmStateAction.TagMatches(this.collideTag, other))
			{
				if (this.storeCollider != null)
				{
					this.storeCollider.Value = other;
				}
				this.storeForce.Value = 0f;
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006E71 RID: 28273 RVA: 0x00223511 File Offset: 0x00221711
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x04006E0B RID: 28171
		[Tooltip("The GameObject to detect collisions on. Unlike regular MonoBehaviour scripts, PlayMaker lets you easily detect collisions on other objects. This lets you organize your behaviours the way you want.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E0C RID: 28172
		[Tooltip("The type of collision to detect.")]
		public CollisionType collision;

		// Token: 0x04006E0D RID: 28173
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Tags to collide with.")]
		public FsmString collideTag;

		// Token: 0x04006E0E RID: 28174
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04006E0F RID: 28175
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Game Object collided with in a {{Game Object Variable}}.")]
		public FsmGameObject storeCollider;

		// Token: 0x04006E10 RID: 28176
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;

		// Token: 0x04006E11 RID: 28177
		private PlayMakerProxyBase cachedProxy;
	}
}
