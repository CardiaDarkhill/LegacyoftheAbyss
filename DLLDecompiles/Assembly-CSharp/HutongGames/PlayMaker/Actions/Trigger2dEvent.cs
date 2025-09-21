using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE7 RID: 4071
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Trigger2dEvent : FsmStateAction
	{
		// Token: 0x0600700A RID: 28682 RVA: 0x0022A365 File Offset: 0x00228565
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = Trigger2DType.OnTriggerEnter2D;
			this.collideTag = "";
			this.sendEvent = null;
			this.storeCollider = null;
		}

		// Token: 0x0600700B RID: 28683 RVA: 0x0022A394 File Offset: 0x00228594
		public override void OnPreprocess()
		{
			if (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner)
			{
				this.GetProxyComponent();
				return;
			}
			switch (this.trigger)
			{
			case Trigger2DType.OnTriggerEnter2D:
				base.Fsm.HandleTriggerEnter2D = true;
				return;
			case Trigger2DType.OnTriggerStay2D:
				base.Fsm.HandleTriggerStay2D = true;
				return;
			case Trigger2DType.OnTriggerExit2D:
				base.Fsm.HandleTriggerExit2D = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600700C RID: 28684 RVA: 0x0022A3F8 File Offset: 0x002285F8
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

		// Token: 0x0600700D RID: 28685 RVA: 0x0022A449 File Offset: 0x00228649
		public override void OnExit()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return;
			}
			this.RemoveCallback();
			this.gameObject.GameObject.OnChange -= this.UpdateCallback;
		}

		// Token: 0x0600700E RID: 28686 RVA: 0x0022A47B File Offset: 0x0022867B
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x0600700F RID: 28687 RVA: 0x0022A490 File Offset: 0x00228690
		private void GetProxyComponent()
		{
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
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerStay2D>(value);
				return;
			case Trigger2DType.OnTriggerExit2D:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit2D>(value);
				return;
			default:
				return;
			}
		}

		// Token: 0x06007010 RID: 28688 RVA: 0x0022A500 File Offset: 0x00228700
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

		// Token: 0x06007011 RID: 28689 RVA: 0x0022A580 File Offset: 0x00228780
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

		// Token: 0x06007012 RID: 28690 RVA: 0x0022A5FD File Offset: 0x002287FD
		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x06007013 RID: 28691 RVA: 0x0022A610 File Offset: 0x00228810
		public override void DoTriggerEnter2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerEnter2D(other);
			}
		}

		// Token: 0x06007014 RID: 28692 RVA: 0x0022A626 File Offset: 0x00228826
		public override void DoTriggerStay2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerStay2D(other);
			}
		}

		// Token: 0x06007015 RID: 28693 RVA: 0x0022A63C File Offset: 0x0022883C
		public override void DoTriggerExit2D(Collider2D other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerExit2D(other);
			}
		}

		// Token: 0x06007016 RID: 28694 RVA: 0x0022A652 File Offset: 0x00228852
		private void TriggerEnter2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(this.collideTag, other))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007017 RID: 28695 RVA: 0x0022A682 File Offset: 0x00228882
		private void TriggerStay2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(this.collideTag, other))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007018 RID: 28696 RVA: 0x0022A6B3 File Offset: 0x002288B3
		private void TriggerExit2D(Collider2D other)
		{
			if (this.trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(this.collideTag, other))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06007019 RID: 28697 RVA: 0x0022A6E4 File Offset: 0x002288E4
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x04007013 RID: 28691
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007014 RID: 28692
		[Tooltip("The type of trigger event to detect.")]
		public Trigger2DType trigger;

		// Token: 0x04007015 RID: 28693
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04007016 RID: 28694
		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04007017 RID: 28695
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04007018 RID: 28696
		private PlayMakerProxyBase cachedProxy;
	}
}
