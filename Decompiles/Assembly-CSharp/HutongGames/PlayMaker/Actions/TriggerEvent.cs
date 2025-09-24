using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB6 RID: 4022
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect trigger collisions between GameObjects that have RigidBody/Collider components.")]
	public class TriggerEvent : FsmStateAction
	{
		// Token: 0x06006EFA RID: 28410 RVA: 0x00225261 File Offset: 0x00223461
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = TriggerType.OnTriggerEnter;
			this.collideTag = "";
			this.sendEvent = null;
			this.storeCollider = null;
		}

		// Token: 0x06006EFB RID: 28411 RVA: 0x00225290 File Offset: 0x00223490
		public override void OnPreprocess()
		{
			if (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner)
			{
				this.GetProxyComponent();
				return;
			}
			switch (this.trigger)
			{
			case TriggerType.OnTriggerEnter:
				base.Fsm.HandleTriggerEnter = true;
				return;
			case TriggerType.OnTriggerStay:
				base.Fsm.HandleTriggerStay = true;
				return;
			case TriggerType.OnTriggerExit:
				base.Fsm.HandleTriggerExit = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x06006EFC RID: 28412 RVA: 0x002252F4 File Offset: 0x002234F4
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

		// Token: 0x06006EFD RID: 28413 RVA: 0x00225345 File Offset: 0x00223545
		public override void OnExit()
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return;
			}
			this.RemoveCallback();
			this.gameObject.GameObject.OnChange -= this.UpdateCallback;
		}

		// Token: 0x06006EFE RID: 28414 RVA: 0x00225377 File Offset: 0x00223577
		private void UpdateCallback()
		{
			this.RemoveCallback();
			this.GetProxyComponent();
			this.AddCallback();
		}

		// Token: 0x06006EFF RID: 28415 RVA: 0x0022538C File Offset: 0x0022358C
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
			case TriggerType.OnTriggerEnter:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerEnter>(value);
				return;
			case TriggerType.OnTriggerStay:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerStay>(value);
				return;
			case TriggerType.OnTriggerExit:
				this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit>(value);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F00 RID: 28416 RVA: 0x002253FC File Offset: 0x002235FC
		private void AddCallback()
		{
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.trigger)
			{
			case TriggerType.OnTriggerEnter:
				this.cachedProxy.AddTriggerEventCallback(new PlayMakerProxyBase.TriggerEvent(this.TriggerEnter));
				return;
			case TriggerType.OnTriggerStay:
				this.cachedProxy.AddTriggerEventCallback(new PlayMakerProxyBase.TriggerEvent(this.TriggerStay));
				return;
			case TriggerType.OnTriggerExit:
				this.cachedProxy.AddTriggerEventCallback(new PlayMakerProxyBase.TriggerEvent(this.TriggerExit));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F01 RID: 28417 RVA: 0x0022547C File Offset: 0x0022367C
		private void RemoveCallback()
		{
			if (this.cachedProxy == null)
			{
				return;
			}
			switch (this.trigger)
			{
			case TriggerType.OnTriggerEnter:
				this.cachedProxy.RemoveTriggerEventCallback(new PlayMakerProxyBase.TriggerEvent(this.TriggerEnter));
				return;
			case TriggerType.OnTriggerStay:
				this.cachedProxy.RemoveTriggerEventCallback(new PlayMakerProxyBase.TriggerEvent(this.TriggerStay));
				return;
			case TriggerType.OnTriggerExit:
				this.cachedProxy.RemoveTriggerEventCallback(new PlayMakerProxyBase.TriggerEvent(this.TriggerExit));
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F02 RID: 28418 RVA: 0x002254F9 File Offset: 0x002236F9
		private void StoreCollisionInfo(Collider collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x06006F03 RID: 28419 RVA: 0x0022550C File Offset: 0x0022370C
		public override void DoTriggerEnter(Collider other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerEnter(other);
			}
		}

		// Token: 0x06006F04 RID: 28420 RVA: 0x00225522 File Offset: 0x00223722
		public override void DoTriggerStay(Collider other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerStay(other);
			}
		}

		// Token: 0x06006F05 RID: 28421 RVA: 0x00225538 File Offset: 0x00223738
		public override void DoTriggerExit(Collider other)
		{
			if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				this.TriggerExit(other);
			}
		}

		// Token: 0x06006F06 RID: 28422 RVA: 0x0022554E File Offset: 0x0022374E
		private void TriggerEnter(Collider other)
		{
			if (this.trigger == TriggerType.OnTriggerEnter && FsmStateAction.TagMatches(this.collideTag, other))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F07 RID: 28423 RVA: 0x0022557E File Offset: 0x0022377E
		private void TriggerStay(Collider other)
		{
			if (this.trigger == TriggerType.OnTriggerStay && FsmStateAction.TagMatches(this.collideTag, other))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F08 RID: 28424 RVA: 0x002255AF File Offset: 0x002237AF
		private void TriggerExit(Collider other)
		{
			if (this.trigger == TriggerType.OnTriggerExit && FsmStateAction.TagMatches(this.collideTag, other))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006F09 RID: 28425 RVA: 0x002255E0 File Offset: 0x002237E0
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x04006EAD RID: 28333
		[Tooltip("The GameObject to detect trigger events on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EAE RID: 28334
		[Tooltip("The type of trigger event to detect.")]
		public TriggerType trigger;

		// Token: 0x04006EAF RID: 28335
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x04006EB0 RID: 28336
		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04006EB1 RID: 28337
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		// Token: 0x04006EB2 RID: 28338
		private PlayMakerProxyBase cachedProxy;
	}
}
