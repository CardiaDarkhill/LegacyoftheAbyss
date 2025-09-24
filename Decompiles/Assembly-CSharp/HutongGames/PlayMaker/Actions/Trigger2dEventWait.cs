using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FEA RID: 4074
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Trigger2dEventWait : FsmStateAction
	{
		// Token: 0x06007043 RID: 28739 RVA: 0x0022B144 File Offset: 0x00229344
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = Trigger2DType.OnTriggerEnter2D;
			this.collideTag = "";
			this.collideLayer = new FsmInt();
			this.sendEvent = null;
			this.storeCollider = null;
			this.waitFrames = null;
			this.finishEvent = null;
		}

		// Token: 0x06007044 RID: 28740 RVA: 0x0022B198 File Offset: 0x00229398
		public override void OnPreprocess()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				switch (this.trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					base.Fsm.HandleTriggerEnter2D = true;
					break;
				case Trigger2DType.OnTriggerExit2D:
					base.Fsm.HandleTriggerExit2D = true;
					break;
				}
			}
			else
			{
				this.GetProxyComponent();
			}
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007045 RID: 28741 RVA: 0x0022B1FC File Offset: 0x002293FC
		public override void OnEnter()
		{
			this.waitFramesCounter = this.waitFrames.Value;
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

		// Token: 0x06007046 RID: 28742 RVA: 0x0022B26D File Offset: 0x0022946D
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

		// Token: 0x06007047 RID: 28743 RVA: 0x0022B2A5 File Offset: 0x002294A5
		public override void OnFixedUpdate()
		{
			if (this.waitFramesCounter >= 0)
			{
				this.waitFramesCounter--;
				return;
			}
			base.Finish();
			base.Fsm.Event(this.finishEvent);
		}

		// Token: 0x06007048 RID: 28744 RVA: 0x0022B2D6 File Offset: 0x002294D6
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x06007049 RID: 28745 RVA: 0x0022B2EC File Offset: 0x002294EC
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

		// Token: 0x0600704A RID: 28746 RVA: 0x0022B358 File Offset: 0x00229558
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

		// Token: 0x0600704B RID: 28747 RVA: 0x0022B3A0 File Offset: 0x002295A0
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

		// Token: 0x0600704C RID: 28748 RVA: 0x0022B420 File Offset: 0x00229620
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

		// Token: 0x0600704D RID: 28749 RVA: 0x0022B49D File Offset: 0x0022969D
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

		// Token: 0x0600704E RID: 28750 RVA: 0x0022B4D8 File Offset: 0x002296D8
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

		// Token: 0x0600704F RID: 28751 RVA: 0x0022B505 File Offset: 0x00229705
		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x06007050 RID: 28752 RVA: 0x0022B518 File Offset: 0x00229718
		public override void DoTriggerEnter2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerEnter2D(other);
			}
		}

		// Token: 0x06007051 RID: 28753 RVA: 0x0022B52E File Offset: 0x0022972E
		public override void DoTriggerStay2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerStay2D(other);
			}
		}

		// Token: 0x06007052 RID: 28754 RVA: 0x0022B544 File Offset: 0x00229744
		public override void DoTriggerExit2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerExit2D(other);
			}
		}

		// Token: 0x06007053 RID: 28755 RVA: 0x0022B55C File Offset: 0x0022975C
		private void TriggerEnter2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(this.collideTag, other) && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007054 RID: 28756 RVA: 0x0022B5BC File Offset: 0x002297BC
		private void TriggerStay2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(this.collideTag, other) && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007055 RID: 28757 RVA: 0x0022B620 File Offset: 0x00229820
		private void TriggerExit2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(this.collideTag, other) && (other.gameObject.layer == this.collideLayer.Value || this.collideLayer.IsNone))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007056 RID: 28758 RVA: 0x0022B681 File Offset: 0x00229881
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x0400702C RID: 28716
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400702D RID: 28717
		[Tooltip("The type of trigger event to detect.")]
		public Trigger2DType trigger;

		// Token: 0x0400702E RID: 28718
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x0400702F RID: 28719
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		// Token: 0x04007030 RID: 28720
		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04007031 RID: 28721
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04007032 RID: 28722
		[Tooltip("Number of fixed update frames to wait")]
		public FsmInt waitFrames;

		// Token: 0x04007033 RID: 28723
		public FsmEvent finishEvent;

		// Token: 0x04007034 RID: 28724
		private PlayMakerProxyBase cachedProxy;

		// Token: 0x04007035 RID: 28725
		private CustomPlayMakerTriggerStay2D triggerStayEventProxy;

		// Token: 0x04007036 RID: 28726
		private bool addedCallback;

		// Token: 0x04007037 RID: 28727
		private int waitFramesCounter;
	}
}
