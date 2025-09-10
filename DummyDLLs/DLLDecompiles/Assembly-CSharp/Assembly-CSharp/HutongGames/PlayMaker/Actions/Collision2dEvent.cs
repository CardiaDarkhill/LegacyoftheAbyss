using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FBD RID: 4029
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Collision2dEvent : FsmStateAction
	{
		// Token: 0x06006F2A RID: 28458 RVA: 0x00225B39 File Offset: 0x00223D39
		public override void Reset()
		{
			this.collision = Collision2DType.OnCollisionEnter2D;
			this.collideTag = "";
			this.sendEvent = null;
			this.storeCollider = null;
			this.storeForce = null;
		}

		// Token: 0x06006F2B RID: 28459 RVA: 0x00225B68 File Offset: 0x00223D68
		public override void OnPreprocess()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				switch (this.collision)
				{
				case Collision2DType.OnCollisionEnter2D:
					base.Fsm.HandleCollisionEnter2D = true;
					return;
				case Collision2DType.OnCollisionStay2D:
					break;
				case Collision2DType.OnCollisionExit2D:
					base.Fsm.HandleCollisionExit2D = true;
					return;
				case Collision2DType.OnParticleCollision:
					base.Fsm.HandleParticleCollision = true;
					return;
				default:
					return;
				}
			}
			else
			{
				this.GetProxyComponent();
			}
		}

		// Token: 0x06006F2C RID: 28460 RVA: 0x00225BD0 File Offset: 0x00223DD0
		private void RemoveEventProxy()
		{
			if (this.eventProxy != null)
			{
				if (this.responder != null)
				{
					this.eventProxy.Remove(this.responder);
					this.responder = null;
				}
				else
				{
					this.eventProxy.Remove(this);
				}
				this.eventProxy = null;
				return;
			}
			if (this.responder != null)
			{
				this.responder.pendingRemoval = true;
				this.responder = null;
			}
		}

		// Token: 0x06006F2D RID: 28461 RVA: 0x00225C3C File Offset: 0x00223E3C
		public override void OnEnter()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.RemoveEventProxy();
				this.eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
				this.responder = this.eventProxy.Add(this, new Action<Collision2D>(this.DoCollisionStay2D));
				return;
			}
			if (this.cachedProxy == null)
			{
				this.GetProxyComponent();
			}
			this.AddCallback();
			this.gameObject.GameObject.OnChange += this.UpdateCallback;
		}

		// Token: 0x06006F2E RID: 28462 RVA: 0x00225CD0 File Offset: 0x00223ED0
		public override void OnExit()
		{
			this.RemoveEventProxy();
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return;
			}
			this.RemoveCallback();
			this.gameObject.GameObject.OnChange -= this.UpdateCallback;
			if (this.responder != null)
			{
				this.responder.pendingRemoval = true;
				this.responder = null;
			}
		}

		// Token: 0x06006F2F RID: 28463 RVA: 0x00225D2E File Offset: 0x00223F2E
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x06006F30 RID: 28464 RVA: 0x00225D44 File Offset: 0x00223F44
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
			case Collision2DType.OnCollisionEnter2D:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionEnter2D>(value);
				return;
			case Collision2DType.OnCollisionStay2D:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionStay2D>(value);
				return;
			case Collision2DType.OnCollisionExit2D:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionExit2D>(value);
				return;
			case Collision2DType.OnParticleCollision:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerParticleCollision>(value);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F31 RID: 28465 RVA: 0x00225DC4 File Offset: 0x00223FC4
		private void AddCallback()
		{
			if (this.collision == Collision2DType.OnCollisionStay2D)
			{
				this.RemoveEventProxy();
				this.eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
				this.responder = this.eventProxy.Add(this, new Action<Collision2D>(this.DoCollisionStay2D));
				return;
			}
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.collision)
			{
			case Collision2DType.OnCollisionEnter2D:
				this.cachedProxy.AddCollision2DEventCallback(new PlayMakerProxyBase.Collision2DEvent(this.CollisionEnter2D));
				return;
			case Collision2DType.OnCollisionStay2D:
				this.cachedProxy.AddCollision2DEventCallback(new PlayMakerProxyBase.Collision2DEvent(this.CollisionStay2D));
				return;
			case Collision2DType.OnCollisionExit2D:
				this.cachedProxy.AddCollision2DEventCallback(new PlayMakerProxyBase.Collision2DEvent(this.CollisionExit2D));
				return;
			case Collision2DType.OnParticleCollision:
				this.cachedProxy.AddParticleCollisionEventCallback(new PlayMakerProxyBase.ParticleCollisionEvent(this.ParticleCollision));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F32 RID: 28466 RVA: 0x00225EA8 File Offset: 0x002240A8
		private void RemoveCallback()
		{
			if (this.collision == Collision2DType.OnCollisionStay2D)
			{
				this.RemoveEventProxy();
				return;
			}
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.collision)
			{
			case Collision2DType.OnCollisionEnter2D:
				this.cachedProxy.RemoveCollision2DEventCallback(new PlayMakerProxyBase.Collision2DEvent(this.CollisionEnter2D));
				return;
			case Collision2DType.OnCollisionStay2D:
				this.cachedProxy.RemoveCollision2DEventCallback(new PlayMakerProxyBase.Collision2DEvent(this.CollisionStay2D));
				return;
			case Collision2DType.OnCollisionExit2D:
				this.cachedProxy.RemoveCollision2DEventCallback(new PlayMakerProxyBase.Collision2DEvent(this.CollisionExit2D));
				return;
			case Collision2DType.OnParticleCollision:
				this.cachedProxy.RemoveParticleCollisionEventCallback(new PlayMakerProxyBase.ParticleCollisionEvent(this.ParticleCollision));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F33 RID: 28467 RVA: 0x00225F54 File Offset: 0x00224154
		private void StoreCollisionInfo(Collision2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
			this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		// Token: 0x06006F34 RID: 28468 RVA: 0x00225F8B File Offset: 0x0022418B
		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.CollisionEnter2D(collisionInfo);
			}
		}

		// Token: 0x06006F35 RID: 28469 RVA: 0x00225FA1 File Offset: 0x002241A1
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.CollisionStay2D(collisionInfo);
			}
		}

		// Token: 0x06006F36 RID: 28470 RVA: 0x00225FB7 File Offset: 0x002241B7
		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.CollisionExit2D(collisionInfo);
			}
		}

		// Token: 0x06006F37 RID: 28471 RVA: 0x00225FCD File Offset: 0x002241CD
		public override void DoParticleCollision(GameObject other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.ParticleCollision(other);
			}
		}

		// Token: 0x06006F38 RID: 28472 RVA: 0x00225FE3 File Offset: 0x002241E3
		private void CollisionEnter2D(Collision2D collisionInfo)
		{
			if (this.collision == Collision2DType.OnCollisionEnter2D && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F39 RID: 28473 RVA: 0x00226013 File Offset: 0x00224213
		private void CollisionStay2D(Collision2D collisionInfo)
		{
			if (this.collision == Collision2DType.OnCollisionStay2D && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F3A RID: 28474 RVA: 0x00226044 File Offset: 0x00224244
		private void CollisionExit2D(Collision2D collisionInfo)
		{
			if (this.collision == Collision2DType.OnCollisionExit2D && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F3B RID: 28475 RVA: 0x00226078 File Offset: 0x00224278
		private void ParticleCollision(GameObject other)
		{
			if (this.collision == Collision2DType.OnParticleCollision && FsmStateAction.TagMatches(this.collideTag, other))
			{
				if (this.storeCollider != null)
				{
					this.storeCollider.Value = other;
				}
				this.storeForce.Value = 0f;
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F3C RID: 28476 RVA: 0x002260D1 File Offset: 0x002242D1
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x04006ECB RID: 28363
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006ECC RID: 28364
		[Tooltip("The type of collision to detect.")]
		public Collision2DType collision;

		// Token: 0x04006ECD RID: 28365
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04006ECE RID: 28366
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04006ECF RID: 28367
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04006ED0 RID: 28368
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision 2D Info to get more info about the collision.")]
		public FsmFloat storeForce;

		// Token: 0x04006ED1 RID: 28369
		private PlayMakerProxyBase cachedProxy;

		// Token: 0x04006ED2 RID: 28370
		private CustomPlayMakerCollisionStay2D eventProxy;

		// Token: 0x04006ED3 RID: 28371
		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;
	}
}
