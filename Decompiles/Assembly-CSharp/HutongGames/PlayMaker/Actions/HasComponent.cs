using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EBB RID: 3771
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Checks if an Object has a Component. Optionally remove the Component on exiting the state.")]
	public class HasComponent : FsmStateAction
	{
		// Token: 0x06006AA1 RID: 27297 RVA: 0x00214686 File Offset: 0x00212886
		public override void Reset()
		{
			this.aComponent = null;
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.component = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006AA2 RID: 27298 RVA: 0x002146B9 File Offset: 0x002128B9
		public override void OnEnter()
		{
			this.DoHasComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006AA3 RID: 27299 RVA: 0x002146F4 File Offset: 0x002128F4
		public override void OnUpdate()
		{
			this.DoHasComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
		}

		// Token: 0x06006AA4 RID: 27300 RVA: 0x00214721 File Offset: 0x00212921
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.aComponent != null)
			{
				Object.Destroy(this.aComponent);
			}
		}

		// Token: 0x06006AA5 RID: 27301 RVA: 0x0021474C File Offset: 0x0021294C
		private void DoHasComponent(GameObject go)
		{
			if (go == null)
			{
				if (!this.store.IsNone)
				{
					this.store.Value = false;
				}
				base.Fsm.Event(this.falseEvent);
				return;
			}
			this.aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(this.component.Value));
			if (!this.store.IsNone)
			{
				this.store.Value = (this.aComponent != null);
			}
			base.Fsm.Event((this.aComponent != null) ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x040069E8 RID: 27112
		[RequiredField]
		[Tooltip("The Game Object to check.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069E9 RID: 27113
		[RequiredField]
		[UIHint(UIHint.ScriptComponent)]
		[Tooltip("The name of the component to check for.")]
		public FsmString component;

		// Token: 0x040069EA RID: 27114
		[Tooltip("Remove the component on exiting the state.")]
		public FsmBool removeOnExit;

		// Token: 0x040069EB RID: 27115
		[Tooltip("Event to send if the Game Object has the component.")]
		public FsmEvent trueEvent;

		// Token: 0x040069EC RID: 27116
		[Tooltip("Event to send if the Game Object does not have the component.")]
		public FsmEvent falseEvent;

		// Token: 0x040069ED RID: 27117
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool store;

		// Token: 0x040069EE RID: 27118
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040069EF RID: 27119
		private Component aComponent;
	}
}
