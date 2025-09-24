using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200114D RID: 4429
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the focused state of a UI InputField component.")]
	public class UiInputFieldGetIsFocused : ComponentAction<InputField>
	{
		// Token: 0x06007721 RID: 30497 RVA: 0x0024471C File Offset: 0x0024291C
		public override void Reset()
		{
			this.isFocused = null;
			this.isfocusedEvent = null;
			this.isNotFocusedEvent = null;
		}

		// Token: 0x06007722 RID: 30498 RVA: 0x00244734 File Offset: 0x00242934
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

		// Token: 0x06007723 RID: 30499 RVA: 0x00244774 File Offset: 0x00242974
		private void DoGetValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			this.isFocused.Value = this.inputField.isFocused;
			base.Fsm.Event(this.inputField.isFocused ? this.isfocusedEvent : this.isNotFocusedEvent);
		}

		// Token: 0x04007798 RID: 30616
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007799 RID: 30617
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the is focused flag value of the UI InputField component.")]
		public FsmBool isFocused;

		// Token: 0x0400779A RID: 30618
		[Tooltip("Event sent if inputField is focused")]
		public FsmEvent isfocusedEvent;

		// Token: 0x0400779B RID: 30619
		[Tooltip("Event sent if nputField is not focused")]
		public FsmEvent isNotFocusedEvent;

		// Token: 0x0400779C RID: 30620
		private InputField inputField;
	}
}
