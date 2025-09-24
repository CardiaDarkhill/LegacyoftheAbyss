using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200114C RID: 4428
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the Hide Mobile Input value of a UI InputField component.")]
	public class UiInputFieldGetHideMobileInput : ComponentAction<InputField>
	{
		// Token: 0x0600771D RID: 30493 RVA: 0x00244665 File Offset: 0x00242865
		public override void Reset()
		{
			this.hideMobileInput = null;
			this.mobileInputHiddenEvent = null;
			this.mobileInputShownEvent = null;
		}

		// Token: 0x0600771E RID: 30494 RVA: 0x0024467C File Offset: 0x0024287C
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

		// Token: 0x0600771F RID: 30495 RVA: 0x002446BC File Offset: 0x002428BC
		private void DoGetValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			this.hideMobileInput.Value = this.inputField.shouldHideMobileInput;
			base.Fsm.Event(this.inputField.shouldHideMobileInput ? this.mobileInputHiddenEvent : this.mobileInputShownEvent);
		}

		// Token: 0x04007793 RID: 30611
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007794 RID: 30612
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Hide Mobile flag value of the UI InputField component.")]
		public FsmBool hideMobileInput;

		// Token: 0x04007795 RID: 30613
		[Tooltip("Event sent if hide mobile input property is true")]
		public FsmEvent mobileInputHiddenEvent;

		// Token: 0x04007796 RID: 30614
		[Tooltip("Event sent if hide mobile input property is false")]
		public FsmEvent mobileInputShownEvent;

		// Token: 0x04007797 RID: 30615
		private InputField inputField;
	}
}
