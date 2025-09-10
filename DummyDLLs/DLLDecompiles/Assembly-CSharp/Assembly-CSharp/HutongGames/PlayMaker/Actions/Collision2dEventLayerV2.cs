using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF9 RID: 3065
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, COLLISION ENTER 2D, COLLISION STAY 2D, and COLLISION EXIT 2D are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
	public class Collision2dEventLayerV2 : FsmStateAction
	{
		// Token: 0x06005DBA RID: 23994 RVA: 0x001D8DA4 File Offset: 0x001D6FA4
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
			this.ignoreFloor = false;
			this.ignoreRoof = false;
			this.onlyAbove = false;
			this.onlyLeft = false;
			this.onlyRight = false;
		}

		// Token: 0x06005DBB RID: 23995 RVA: 0x001D8E04 File Offset: 0x001D7004
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

		// Token: 0x06005DBC RID: 23996 RVA: 0x001D8EA8 File Offset: 0x001D70A8
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

		// Token: 0x06005DBD RID: 23997 RVA: 0x001D8F28 File Offset: 0x001D7128
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

		// Token: 0x06005DBE RID: 23998 RVA: 0x001D8FB4 File Offset: 0x001D71B4
		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				float x = collisionInfo.contacts[0].normal.x;
				float y = collisionInfo.contacts[0].normal.y;
				if (this.CheckConditions(x, y))
				{
					base.Fsm.Event(this.sendEvent);
				}
			}
		}

		// Token: 0x06005DBF RID: 23999 RVA: 0x001D9084 File Offset: 0x001D7284
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionStay2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				float x = collisionInfo.contacts[0].normal.x;
				float y = collisionInfo.contacts[0].normal.y;
				if (this.CheckConditions(x, y))
				{
					base.Fsm.Event(this.sendEvent);
				}
			}
		}

		// Token: 0x06005DC0 RID: 24000 RVA: 0x001D9154 File Offset: 0x001D7354
		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (this.collision == PlayMakerUnity2d.Collision2DType.OnCollisionExit2D && (collisionInfo.collider.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06005DC1 RID: 24001 RVA: 0x001D91E4 File Offset: 0x001D73E4
		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (base.Owner != null && base.Owner.GetComponent<Collider2D>() == null && base.Owner.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}

		// Token: 0x06005DC2 RID: 24002 RVA: 0x001D9238 File Offset: 0x001D7438
		private bool CheckConditions(float normalX, float normalY)
		{
			return (!this.ignoreFloor || !Mathf.Approximately(normalY, 1f)) && (!this.ignoreRoof || !Mathf.Approximately(normalY, -1f)) && (!this.onlyAbove || Mathf.Approximately(normalY, -1f)) && (!this.onlyLeft || Mathf.Approximately(normalX, 1f)) && (!this.onlyRight || Mathf.Approximately(normalX, -1f));
		}

		// Token: 0x04005A0A RID: 23050
		[Tooltip("The type of collision to detect.")]
		public PlayMakerUnity2d.Collision2DType collision;

		// Token: 0x04005A0B RID: 23051
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04005A0C RID: 23052
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x04005A0D RID: 23053
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04005A0E RID: 23054
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04005A0F RID: 23055
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;

		// Token: 0x04005A10 RID: 23056
		[UIHint(UIHint.Variable)]
		public FsmVector3 contactPoint;

		// Token: 0x04005A11 RID: 23057
		[UIHint(UIHint.Variable)]
		public FsmVector3 contactNormal;

		// Token: 0x04005A12 RID: 23058
		public bool ignoreFloor;

		// Token: 0x04005A13 RID: 23059
		public bool ignoreRoof;

		// Token: 0x04005A14 RID: 23060
		public bool onlyAbove;

		// Token: 0x04005A15 RID: 23061
		public bool onlyLeft;

		// Token: 0x04005A16 RID: 23062
		public bool onlyRight;

		// Token: 0x04005A17 RID: 23063
		private PlayMakerUnity2DProxy _proxy;
	}
}
