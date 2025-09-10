using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200115E RID: 4446
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the selection color of a UI InputField component. This is the color of the highlighter to show what characters are selected.")]
	public class UiInputFieldSetSelectionColor : ComponentAction<InputField>
	{
		// Token: 0x06007774 RID: 30580 RVA: 0x002454ED File Offset: 0x002436ED
		public override void Reset()
		{
			this.gameObject = null;
			this.selectionColor = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x06007775 RID: 30581 RVA: 0x0024550C File Offset: 0x0024370C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalValue = this.inputField.selectionColor;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007776 RID: 30582 RVA: 0x00245565 File Offset: 0x00243765
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x06007777 RID: 30583 RVA: 0x0024556D File Offset: 0x0024376D
		private void DoSetValue()
		{
			if (this.inputField != null)
			{
				this.inputField.selectionColor = this.selectionColor.Value;
			}
		}

		// Token: 0x06007778 RID: 30584 RVA: 0x00245593 File Offset: 0x00243793
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.selectionColor = this.originalValue;
			}
		}

		// Token: 0x040077F4 RID: 30708
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077F5 RID: 30709
		[RequiredField]
		[Tooltip("The color of the highlighter to show what characters are selected for the UI InputField component.")]
		public FsmColor selectionColor;

		// Token: 0x040077F6 RID: 30710
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077F7 RID: 30711
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040077F8 RID: 30712
		private InputField inputField;

		// Token: 0x040077F9 RID: 30713
		private Color originalValue;
	}
}
