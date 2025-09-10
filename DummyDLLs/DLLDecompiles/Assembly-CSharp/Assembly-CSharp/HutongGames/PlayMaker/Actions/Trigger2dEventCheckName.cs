using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE8 RID: 4072
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Trigger2dEventCheckName : FsmStateAction
	{
		// Token: 0x0600701B RID: 28699 RVA: 0x0022A704 File Offset: 0x00228904
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = Trigger2DType.OnTriggerEnter2D;
			this.collideTag = "";
			this.collideLayer = new FsmInt();
			this.sendEvent = null;
			this.storeCollider = null;
			this.requiredGameObjectName = null;
		}

		// Token: 0x0600701C RID: 28700 RVA: 0x0022A744 File Offset: 0x00228944
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

		// Token: 0x0600701D RID: 28701 RVA: 0x0022A798 File Offset: 0x00228998
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

		// Token: 0x0600701E RID: 28702 RVA: 0x0022A7F8 File Offset: 0x002289F8
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

		// Token: 0x0600701F RID: 28703 RVA: 0x0022A830 File Offset: 0x00228A30
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x06007020 RID: 28704 RVA: 0x0022A844 File Offset: 0x00228A44
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

		// Token: 0x06007021 RID: 28705 RVA: 0x0022A8B0 File Offset: 0x00228AB0
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

		// Token: 0x06007022 RID: 28706 RVA: 0x0022A8F8 File Offset: 0x00228AF8
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

		// Token: 0x06007023 RID: 28707 RVA: 0x0022A978 File Offset: 0x00228B78
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

		// Token: 0x06007024 RID: 28708 RVA: 0x0022A9F5 File Offset: 0x00228BF5
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

		// Token: 0x06007025 RID: 28709 RVA: 0x0022AA30 File Offset: 0x00228C30
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

		// Token: 0x06007026 RID: 28710 RVA: 0x0022AA5D File Offset: 0x00228C5D
		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x06007027 RID: 28711 RVA: 0x0022AA70 File Offset: 0x00228C70
		public override void DoTriggerEnter2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerEnter2D(other);
			}
		}

		// Token: 0x06007028 RID: 28712 RVA: 0x0022AA86 File Offset: 0x00228C86
		public override void DoTriggerStay2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerStay2D(other);
			}
		}

		// Token: 0x06007029 RID: 28713 RVA: 0x0022AA9C File Offset: 0x00228C9C
		public override void DoTriggerExit2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerExit2D(other);
			}
		}

		// Token: 0x0600702A RID: 28714 RVA: 0x0022AAB4 File Offset: 0x00228CB4
		private void TriggerEnter2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(this.collideTag, other) && other.gameObject.name == this.requiredGameObjectName.Value && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600702B RID: 28715 RVA: 0x0022AB34 File Offset: 0x00228D34
		private void TriggerStay2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(this.collideTag, other) && other.gameObject.name == this.requiredGameObjectName.Value && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600702C RID: 28716 RVA: 0x0022ABB4 File Offset: 0x00228DB4
		private void TriggerExit2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(this.collideTag, other) && other.gameObject.name == this.requiredGameObjectName.Value && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x0600702D RID: 28717 RVA: 0x0022AC32 File Offset: 0x00228E32
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x04007019 RID: 28697
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400701A RID: 28698
		[Tooltip("The type of trigger event to detect.")]
		public Trigger2DType trigger;

		// Token: 0x0400701B RID: 28699
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x0400701C RID: 28700
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x0400701D RID: 28701
		[Tooltip("Filter by GameObject name.")]
		public FsmString requiredGameObjectName;

		// Token: 0x0400701E RID: 28702
		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x0400701F RID: 28703
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04007020 RID: 28704
		private PlayMakerProxyBase cachedProxy;

		// Token: 0x04007021 RID: 28705
		private CustomPlayMakerTriggerStay2D triggerStayEventProxy;

		// Token: 0x04007022 RID: 28706
		private bool addedCallback;
	}
}
