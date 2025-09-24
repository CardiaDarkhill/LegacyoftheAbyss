using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200114E RID: 4430
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the placeHolder GameObject of a UI InputField component.")]
	public class UiInputFieldGetPlaceHolder : ComponentAction<InputField>
	{
		// Token: 0x06007725 RID: 30501 RVA: 0x002447D4 File Offset: 0x002429D4
		public override void Reset()
		{
			this.placeHolder = null;
			this.placeHolderDefined = null;
			this.foundEvent = null;
			this.notFoundEvent = null;
		}

		// Token: 0x06007726 RID: 30502 RVA: 0x002447F4 File Offset: 0x002429F4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x06007727 RID: 30503 RVA: 0x00244834 File Offset: 0x00242A34
		private void DoGetValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			Graphic placeholder = this.inputField.placeholder;
			this.placeHolderDefined.Value = (placeholder != null);
			if (placeholder != null)
			{
				this.placeHolder.Value = placeholder.gameObject;
				base.Fsm.Event(this.foundEvent);
				return;
			}
			base.Fsm.Event(this.notFoundEvent);
		}

		// Token: 0x0400779D RID: 30621
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400779E RID: 30622
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the placeholder for the UI InputField component.")]
		public FsmGameObject placeHolder;

		// Token: 0x0400779F RID: 30623
		[Tooltip("true if placeholder is found")]
		public FsmBool placeHolderDefined;

		// Token: 0x040077A0 RID: 30624
		[Tooltip("Event sent if no placeholder is defined")]
		public FsmEvent foundEvent;

		// Token: 0x040077A1 RID: 30625
		[Tooltip("Event sent if a placeholder is defined")]
		public FsmEvent notFoundEvent;

		// Token: 0x040077A2 RID: 30626
		private InputField inputField;
	}
}
