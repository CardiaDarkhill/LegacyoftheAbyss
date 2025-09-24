using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B16 RID: 2838
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, COLLISION ENTER 2D, COLLISION STAY 2D, and COLLISION EXIT 2D are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
	public class Collision2dEventLayer : FsmStateAction
	{
		// Token: 0x06005946 RID: 22854 RVA: 0x001C4A1A File Offset: 0x001C2C1A
		public override void Reset()
		{
			this.collision = PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D;
			this.collideTag = new FsmString
			{
				UseVariable = true
			};
			this.sendEvent = null;
			this.storeCollider = null;
			this.storeForce = null;
		}

		// Token: 0x06005947 RID: 22855 RVA: 0x001C4A4C File Offset: 0x001C2C4C
		public override void OnEnter()
		{
			if (this._proxy == null)
			{
				this._proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
				if (this._proxy == null)
				{
					this._proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
				}
			}
			switch (this.collision)
			{
			case PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D:
				this._proxy.AddOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(this.DoCollisionEnter2D));
				return;
			case PlayMakerUnity2d.Collision2DType.OnCollisionStay2D:
				this._proxy.AddOnCollisionStay2dDelegate(new PlayMakerUnity2DProxy.OnCollisionStay2dDelegate(this.DoCollisionStay2D));
				return;
			case PlayMakerUnity2d.Collision2DType.OnCollisionExit2D:
				this._proxy.AddOnCollisionExit2dDelegate(new PlayMakerUnity2DProxy.OnCollisionExit2dDelegate(this.DoCollisionExit2D));
				return;
			default:
				return;
			}
		}

		// Token: 0x06005948 RID: 22856 RVA: 0x001C4AFC File Offset: 0x001C2CFC
		public override void OnExit()
		{
			if (this._proxy == null)
			{
				return;
			}
			switch (this.collision)
			{
			case PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D:
				this._proxy.RemoveOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(this.DoCollisionEnter2D));
				return;
			case PlayMakerUnity2d.Collision2DType.OnCollisionStay2D:
				this._proxy.RemoveOnCollisionStay2dDelegate(new PlayMakerUnity2DProxy.OnCollisionStay2dDelegate(this.DoCollisionStay2D));
				return;
			case PlayMakerUnity2d.Collision2DType.OnCollisionExit2D:
				this._proxy.RemoveOnCollisionExit2dDelegate(new PlayMakerUnity2DProxy.OnCollisionExit2dDelegate(this.DoCollisionExit2D));
				return;
			default:
				return;
			}
		}

		// Token: 0x06005949 RID: 22857 RVA: 0x001C4B7C File Offset: 0x001C2D7C
		private void StoreCollisionInfo(Collision2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
			this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
			if (collisionInfo.contacts != null && collisionInfo.contacts.Length != 0)
			{
				this.contactPoint.Value = collisionInfo.contacts[0].point;
				this.contactNormal.Value = collisionInfo.contacts[0].normal;
			}
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x001C4C08 File Offset: 0x001C2E08
		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x001C4C9C File Offset: 0x001C2E9C
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionStay2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600594C RID: 22860 RVA: 0x001C4D30 File Offset: 0x001C2F30
		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionExit2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600594D RID: 22861 RVA: 0x001C4DC4 File Offset: 0x001C2FC4
		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (base.Owner != null && base.Owner.GetComponent<Collider2D>() == null && base.Owner.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}

		// Token: 0x040054A4 RID: 21668
		[Tooltip("The type of collision to detect.")]
		public PlayMakerUnity2d.Collision2DType collision;

		// Token: 0x040054A5 RID: 21669
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x040054A6 RID: 21670
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x040054A7 RID: 21671
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x040054A8 RID: 21672
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x040054A9 RID: 21673
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;

		// Token: 0x040054AA RID: 21674
		[UIHint(UIHint.Variable)]
		public FsmVector3 contactPoint;

		// Token: 0x040054AB RID: 21675
		[UIHint(UIHint.Variable)]
		public FsmVector3 contactNormal;

		// Token: 0x040054AC RID: 21676
		private PlayMakerUnity2DProxy _proxy;
	}
}
