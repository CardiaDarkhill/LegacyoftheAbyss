using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1B RID: 3355
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D trigger collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, TRIGGER ENTER 2D, TRIGGER STAY 2D, and TRIGGER EXIT 2D are sent automatically on collisions triggers with any object. Use this action to filter collision triggers by Tag.")]
	public class SendTrigger2DEvent : FsmStateAction
	{
		// Token: 0x060062F7 RID: 25335 RVA: 0x001F4C38 File Offset: 0x001F2E38
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

		// Token: 0x060062F8 RID: 25336 RVA: 0x001F4C74 File Offset: 0x001F2E74
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

		// Token: 0x060062F9 RID: 25337 RVA: 0x001F4D18 File Offset: 0x001F2F18
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

		// Token: 0x060062FA RID: 25338 RVA: 0x001F4D98 File Offset: 0x001F2F98
		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x001F4DAC File Offset: 0x001F2FAC
		public override void DoTriggerEnter2D(Collider2D collisionInfo)
		{
			if (this.trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D && (collisionInfo.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.eventTarget, this.sendEvent);
			}
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x001F4E40 File Offset: 0x001F3040
		public override void DoTriggerStay2D(Collider2D collisionInfo)
		{
			if (this.trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D && (collisionInfo.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.eventTarget, this.sendEvent);
			}
		}

		// Token: 0x060062FD RID: 25341 RVA: 0x001F4ED8 File Offset: 0x001F30D8
		public override void DoTriggerExit2D(Collider2D collisionInfo)
		{
			if (this.trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D && (collisionInfo.gameObject.tag == this.collideTag.Value || this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value)) && (collisionInfo.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.eventTarget, this.sendEvent);
			}
		}

		// Token: 0x060062FE RID: 25342 RVA: 0x001F4F70 File Offset: 0x001F3170
		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (base.Owner != null && base.Owner.GetComponent<Collider2D>() == null && base.Owner.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}

		// Token: 0x04006172 RID: 24946
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04006173 RID: 24947
		[Tooltip("The type of trigger to detect.")]
		public PlayMakerUnity2d.Trigger2DType trigger;

		// Token: 0x04006174 RID: 24948
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04006175 RID: 24949
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x04006176 RID: 24950
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04006177 RID: 24951
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04006178 RID: 24952
		private PlayMakerUnity2DProxy _proxy;
	}
}
