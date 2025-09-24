using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001131 RID: 4401
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the interactable flag of a UI Selectable component.")]
	public class UiGetIsInteractable : ComponentAction<Selectable>
	{
		// Token: 0x0600769D RID: 30365 RVA: 0x00242B76 File Offset: 0x00240D76
		public override void Reset()
		{
			this.gameObject = null;
			this.isInteractable = null;
			this.isInteractableEvent = null;
			this.isNotInteractableEvent = null;
		}

		// Token: 0x0600769E RID: 30366 RVA: 0x00242B94 File Offset: 0x00240D94
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x0600769F RID: 30367 RVA: 0x00242BD4 File Offset: 0x00240DD4
		private void DoGetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			bool flag = this.selectable.IsInteractable();
			this.isInteractable.Value = flag;
			base.Fsm.Event(flag ? this.isInteractableEvent : this.isNotInteractableEvent);
		}

		// Token: 0x040076FD RID: 30461
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040076FE RID: 30462
		[Tooltip("The Interactable value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isInteractable;

		// Token: 0x040076FF RID: 30463
		[Tooltip("Event sent if Component is Interactable")]
		public FsmEvent isInteractableEvent;

		// Token: 0x04007700 RID: 30464
		[Tooltip("Event sent if Component is not Interactable")]
		public FsmEvent isNotInteractableEvent;

		// Token: 0x04007701 RID: 30465
		private Selectable selectable;
	}
}
