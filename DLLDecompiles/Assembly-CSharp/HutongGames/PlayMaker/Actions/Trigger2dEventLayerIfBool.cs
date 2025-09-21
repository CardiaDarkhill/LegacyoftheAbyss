using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA1 RID: 3489
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D trigger collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, TRIGGER ENTER 2D, TRIGGER STAY 2D, and TRIGGER EXIT 2D are sent automatically on collisions triggers with any object. Use this action to filter collision triggers by Tag.")]
	public class Trigger2dEventLayerIfBool : FsmStateAction
	{
		// Token: 0x0600654E RID: 25934 RVA: 0x001FF721 File Offset: 0x001FD921
		public override void Reset()
		{
			this.trigger = PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D;
			this.collideTag = new FsmString
			{
				UseVariable = true
			};
			this.collideLayer = new FsmInt
			{
				UseVariable = true
			};
			this.sendEvent = null;
			this.storeCollider = null;
		}

		// Token: 0x0600654F RID: 25935 RVA: 0x001FF75C File Offset: 0x001FD95C
		public override void OnEnter()
		{
			this._proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
			if (this._proxy == null)
			{
				this._proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
			}
			switch (this.trigger)
			{
			case PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D:
				this._proxy.AddOnTriggerEnter2dDelegate(new PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate(this.DoTriggerEnter2D));
				return;
			case PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D:
				this._proxy.AddOnTriggerStay2dDelegate(new PlayMakerUnity2DProxy.OnTriggerStay2dDelegate(this.DoTriggerStay2D));
				return;
			case PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D:
				this._proxy.AddOnTriggerExit2dDelegate(new PlayMakerUnity2DProxy.OnTriggerExit2dDelegate(this.DoTriggerExit2D));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006550 RID: 25936 RVA: 0x001FF800 File Offset: 0x001FDA00
		public override void OnExit()
		{
			if (this._proxy == null)
			{
				return;
			}
			switch (this.trigger)
			{
			case PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D:
				this._proxy.RemoveOnTriggerEnter2dDelegate(new PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate(this.DoTriggerEnter2D));
				return;
			case PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D:
				this._proxy.RemoveOnTriggerStay2dDelegate(new PlayMakerUnity2DProxy.OnTriggerStay2dDelegate(this.DoTriggerStay2D));
				return;
			case PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D:
				this._proxy.RemoveOnTriggerExit2dDelegate(new PlayMakerUnity2DProxy.OnTriggerExit2dDelegate(this.DoTriggerExit2D));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006551 RID: 25937 RVA: 0x001FF880 File Offset: 0x001FDA80
		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x06006552 RID: 25938 RVA: 0x001FF894 File Offset: 0x001FDA94
		public override void DoTriggerEnter2D(Collider2D collisionInfo)
		{
			if (this.trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D && (collisionInfo.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				if (this.activeBool.Value)
				{
					base.Fsm.Event(this.sendEvent);
				}
			}
		}

		// Token: 0x06006553 RID: 25939 RVA: 0x001FF934 File Offset: 0x001FDB34
		public override void DoTriggerStay2D(Collider2D collisionInfo)
		{
			if (this.trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D && (collisionInfo.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				if (this.activeBool.Value)
				{
					base.Fsm.Event(this.sendEvent);
				}
			}
		}

		// Token: 0x06006554 RID: 25940 RVA: 0x001FF9D4 File Offset: 0x001FDBD4
		public override void DoTriggerExit2D(Collider2D collisionInfo)
		{
			if (this.trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D && (collisionInfo.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				if (this.activeBool.Value)
				{
					base.Fsm.Event(this.sendEvent);
				}
			}
		}

		// Token: 0x06006555 RID: 25941 RVA: 0x001FFA74 File Offset: 0x001FDC74
		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (base.Owner != null && base.Owner.GetComponent<Collider2D>() == null && base.Owner.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}

		// Token: 0x0400644F RID: 25679
		[Tooltip("The type of trigger to detect.")]
		public PlayMakerUnity2d.Trigger2DType trigger;

		// Token: 0x04006450 RID: 25680
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04006451 RID: 25681
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x04006452 RID: 25682
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04006453 RID: 25683
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04006454 RID: 25684
		public FsmBool activeBool;

		// Token: 0x04006455 RID: 25685
		private PlayMakerUnity2DProxy _proxy;
	}
}
