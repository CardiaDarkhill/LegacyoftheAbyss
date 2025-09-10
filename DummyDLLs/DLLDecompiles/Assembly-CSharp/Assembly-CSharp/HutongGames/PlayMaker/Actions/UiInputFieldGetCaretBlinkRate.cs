using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200114A RID: 4426
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the caret's blink rate of a UI InputField component.")]
	public class UiInputFieldGetCaretBlinkRate : ComponentAction<InputField>
	{
		// Token: 0x06007713 RID: 30483 RVA: 0x00244514 File Offset: 0x00242714
		public override void Reset()
		{
			this.caretBlinkRate = null;
			this.everyFrame = false;
		}

		// Token: 0x06007714 RID: 30484 RVA: 0x00244524 File Offset: 0x00242724
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007715 RID: 30485 RVA: 0x0024456C File Offset: 0x0024276C
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x06007716 RID: 30486 RVA: 0x00244574 File Offset: 0x00242774
		private void DoGetValue()
		{
			if (this.inputField != null)
			{
				this.caretBlinkRate.Value = this.inputField.caretBlinkRate;
			}
		}

		// Token: 0x04007789 RID: 30601
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400778A RID: 30602
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The caret's blink rate for the UI InputField component.")]
		public FsmFloat caretBlinkRate;

		// Token: 0x0400778B RID: 30603
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x0400778C RID: 30604
		private InputField inputField;
	}
}
