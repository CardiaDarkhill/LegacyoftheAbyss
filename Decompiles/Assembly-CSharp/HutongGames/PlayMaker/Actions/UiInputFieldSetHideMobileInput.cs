using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200115C RID: 4444
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the Hide Mobile Input property of a UI InputField component.")]
	public class UiInputFieldSetHideMobileInput : ComponentAction<InputField>
	{
		// Token: 0x0600776A RID: 30570 RVA: 0x00245332 File Offset: 0x00243532
		public override void Reset()
		{
			this.gameObject = null;
			this.hideMobileInput = null;
			this.resetOnExit = null;
		}

		// Token: 0x0600776B RID: 30571 RVA: 0x0024534C File Offset: 0x0024354C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalValue = this.inputField.shouldHideMobileInput;
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x0600776C RID: 30572 RVA: 0x0024539D File Offset: 0x0024359D
		private void DoSetValue()
		{
			if (this.inputField != null)
			{
				this.inputField.shouldHideMobileInput = this.hideMobileInput.Value;
			}
		}

		// Token: 0x0600776D RID: 30573 RVA: 0x002453C3 File Offset: 0x002435C3
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.shouldHideMobileInput = this.originalValue;
			}
		}

		// Token: 0x040077EA RID: 30698
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077EB RID: 30699
		[RequiredField]
		[UIHint(UIHint.TextArea)]
		[Tooltip("The Hide Mobile Input flag value of the UI InputField component.")]
		public FsmBool hideMobileInput;

		// Token: 0x040077EC RID: 30700
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077ED RID: 30701
		private InputField inputField;

		// Token: 0x040077EE RID: 30702
		private bool originalValue;
	}
}
