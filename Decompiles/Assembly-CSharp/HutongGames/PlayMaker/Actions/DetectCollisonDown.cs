using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C13 RID: 3091
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, COLLISION ENTER 2D, COLLISION STAY 2D, and COLLISION EXIT 2D are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
	public class DetectCollisonDown : FsmStateAction
	{
		// Token: 0x06005E39 RID: 24121 RVA: 0x001DB014 File Offset: 0x001D9214
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

		// Token: 0x06005E3A RID: 24122 RVA: 0x001DB044 File Offset: 0x001D9244
		public override void OnEnter()
		{
			this._proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
			if (this._proxy == null)
			{
				this._proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
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

		// Token: 0x06005E3B RID: 24123 RVA: 0x001DB0E8 File Offset: 0x001D92E8
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

		// Token: 0x06005E3C RID: 24124 RVA: 0x001DB168 File Offset: 0x001D9368
		private void StoreCollisionInfo(Collision2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
			this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		// Token: 0x06005E3D RID: 24125 RVA: 0x001DB1A0 File Offset: 0x001D93A0
		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06005E3E RID: 24126 RVA: 0x001DB210 File Offset: 0x001D9410
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionStay2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06005E3F RID: 24127 RVA: 0x001DB280 File Offset: 0x001D9480
		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionExit2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06005E40 RID: 24128 RVA: 0x001DB2F0 File Offset: 0x001D94F0
		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (base.Owner != null && base.Owner.GetComponent<Collider2D>() == null && base.Owner.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}

		// Token: 0x04005A84 RID: 23172
		[Tooltip("The type of collision to detect.")]
		public PlayMakerUnity2d.Collision2DType collision;

		// Token: 0x04005A85 RID: 23173
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04005A86 RID: 23174
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04005A87 RID: 23175
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04005A88 RID: 23176
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;

		// Token: 0x04005A89 RID: 23177
		private PlayMakerUnity2DProxy _proxy;
	}
}
