using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE9 RID: 4073
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Trigger2dEventV2 : FsmStateAction
	{
		// Token: 0x0600702F RID: 28719 RVA: 0x0022AC52 File Offset: 0x00228E52
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = Trigger2DType.OnTriggerEnter2D;
			this.collideTag = "";
			this.collideLayer = new FsmInt();
			this.sendEvent = null;
			this.storeCollider = null;
		}

		// Token: 0x06007030 RID: 28720 RVA: 0x0022AC8C File Offset: 0x00228E8C
		public override void OnPreprocess()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				switch (this.trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					base.Fsm.HandleTriggerEnter2D = true;
					return;
				case Trigger2DType.OnTriggerStay2D:
					break;
				case Trigger2DType.OnTriggerExit2D:
					base.Fsm.HandleTriggerExit2D = true;
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

		// Token: 0x06007031 RID: 28721 RVA: 0x0022ACE0 File Offset: 0x00228EE0
		public override void OnEnter()
		{
			if (this.AddCustomProxy())
			{
				this.AddCallbackTriggerStay();
				return;
			}
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

		// Token: 0x06007032 RID: 28722 RVA: 0x0022AD40 File Offset: 0x00228F40
		public override void OnExit()
		{
			this.RemoveCallbackTriggerStay();
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return;
			}
			this.RemoveCallback();
			this.gameObject.GameObject.OnChange -= this.UpdateCallback;
		}

		// Token: 0x06007033 RID: 28723 RVA: 0x0022AD78 File Offset: 0x00228F78
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x06007034 RID: 28724 RVA: 0x0022AD8C File Offset: 0x00228F8C
		private void GetProxyComponent()
		{
			if (this.AddCustomProxy())
			{
				return;
			}
			this.cachedProxy = null;
			GameObject value = this.gameObject.GameObject.Value;
			if (value == null)
			{
				return;
			}
			switch (this.trigger)
			{
			case Trigger2DType.OnTriggerEnter2D:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerEnter2D>(value);
				return;
			case Trigger2DType.OnTriggerStay2D:
				break;
			case Trigger2DType.OnTriggerExit2D:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit2D>(value);
				break;
			default:
				return;
			}
		}

		// Token: 0x06007035 RID: 28725 RVA: 0x0022ADF8 File Offset: 0x00228FF8
		private bool AddCustomProxy()
		{
			if (this.trigger == Trigger2DType.OnTriggerStay2D)
			{
				if (this.triggerStayEventProxy == null)
				{
					GameObject safe = this.gameObject.GetSafe(this);
					if (safe)
					{
						this.triggerStayEventProxy = CustomPlayMakerTriggerStay2D.GetEventSender(safe);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06007036 RID: 28726 RVA: 0x0022AE40 File Offset: 0x00229040
		private void AddCallback()
		{
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.trigger)
			{
			case Trigger2DType.OnTriggerEnter2D:
				this.cachedProxy.AddTrigger2DEventCallback(new PlayMakerProxyBase.Trigger2DEvent(this.TriggerEnter2D));
				return;
			case Trigger2DType.OnTriggerStay2D:
				this.cachedProxy.AddTrigger2DEventCallback(new PlayMakerProxyBase.Trigger2DEvent(this.TriggerStay2D));
				return;
			case Trigger2DType.OnTriggerExit2D:
				this.cachedProxy.AddTrigger2DEventCallback(new PlayMakerProxyBase.Trigger2DEvent(this.TriggerExit2D));
				return;
			default:
				return;
			}
		}

		// Token: 0x06007037 RID: 28727 RVA: 0x0022AEC0 File Offset: 0x002290C0
		private void RemoveCallback()
		{
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.trigger)
			{
			case Trigger2DType.OnTriggerEnter2D:
				this.cachedProxy.RemoveTrigger2DEventCallback(new PlayMakerProxyBase.Trigger2DEvent(this.TriggerEnter2D));
				return;
			case Trigger2DType.OnTriggerStay2D:
				this.cachedProxy.RemoveTrigger2DEventCallback(new PlayMakerProxyBase.Trigger2DEvent(this.TriggerStay2D));
				return;
			case Trigger2DType.OnTriggerExit2D:
				this.cachedProxy.RemoveTrigger2DEventCallback(new PlayMakerProxyBase.Trigger2DEvent(this.TriggerExit2D));
				return;
			default:
				return;
			}
		}

		// Token: 0x06007038 RID: 28728 RVA: 0x0022AF3D File Offset: 0x0022913D
		private void AddCallbackTriggerStay()
		{
			if (this.trigger != Trigger2DType.OnTriggerStay2D)
			{
				return;
			}
			if (this.triggerStayEventProxy == null)
			{
				return;
			}
			this.addedCallback = true;
			this.triggerStayEventProxy.Add(this, new Action<Collider2D>(this.TriggerStay2D));
		}

		// Token: 0x06007039 RID: 28729 RVA: 0x0022AF78 File Offset: 0x00229178
		private void RemoveCallbackTriggerStay()
		{
			if (!this.addedCallback)
			{
				return;
			}
			if (this.triggerStayEventProxy == null)
			{
				return;
			}
			this.addedCallback = false;
			this.triggerStayEventProxy.Remove(this);
		}

		// Token: 0x0600703A RID: 28730 RVA: 0x0022AFA5 File Offset: 0x002291A5
		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x0600703B RID: 28731 RVA: 0x0022AFB8 File Offset: 0x002291B8
		public override void DoTriggerEnter2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerEnter2D(other);
			}
		}

		// Token: 0x0600703C RID: 28732 RVA: 0x0022AFCE File Offset: 0x002291CE
		public override void DoTriggerStay2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerStay2D(other);
			}
		}

		// Token: 0x0600703D RID: 28733 RVA: 0x0022AFE4 File Offset: 0x002291E4
		public override void DoTriggerExit2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerExit2D(other);
			}
		}

		// Token: 0x0600703E RID: 28734 RVA: 0x0022AFFC File Offset: 0x002291FC
		private void TriggerEnter2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(this.collideTag, other) && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600703F RID: 28735 RVA: 0x0022B05C File Offset: 0x0022925C
		private void TriggerStay2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(this.collideTag, other) && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007040 RID: 28736 RVA: 0x0022B0C0 File Offset: 0x002292C0
		private void TriggerExit2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(this.collideTag, other) && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007041 RID: 28737 RVA: 0x0022B121 File Offset: 0x00229321
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x04007023 RID: 28707
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007024 RID: 28708
		[Tooltip("The type of trigger event to detect.")]
		public Trigger2DType trigger;

		// Token: 0x04007025 RID: 28709
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04007026 RID: 28710
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x04007027 RID: 28711
		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04007028 RID: 28712
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04007029 RID: 28713
		private PlayMakerProxyBase cachedProxy;

		// Token: 0x0400702A RID: 28714
		private CustomPlayMakerTriggerStay2D triggerStayEventProxy;

		// Token: 0x0400702B RID: 28715
		private bool addedCallback;
	}
}
