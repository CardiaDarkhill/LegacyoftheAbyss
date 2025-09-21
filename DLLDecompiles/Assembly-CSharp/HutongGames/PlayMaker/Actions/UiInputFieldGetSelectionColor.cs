using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200114F RID: 4431
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the selection color of a UI InputField component. This is the color of the highlighter to show what characters are selected")]
	public class UiInputFieldGetSelectionColor : ComponentAction<InputField>
	{
		// Token: 0x06007729 RID: 30505 RVA: 0x002448B3 File Offset: 0x00242AB3
		public override void Reset()
		{
			this.selectionColor = null;
			this.everyFrame = false;
		}

		// Token: 0x0600772A RID: 30506 RVA: 0x002448C4 File Offset: 0x00242AC4
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

		// Token: 0x0600772B RID: 30507 RVA: 0x0024490C File Offset: 0x00242B0C
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x0600772C RID: 30508 RVA: 0x00244914 File Offset: 0x00242B14
		private void DoGetValue()
		{
			if (this.inputField != null)
			{
				this.selectionColor.Value = this.inputField.selectionColor;
			}
		}

		// Token: 0x040077A3 RID: 30627
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077A4 RID: 30628
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("This is the color of the highlighter to show what characters are selected of the UI InputField component.")]
		public FsmColor selectionColor;

		// Token: 0x040077A5 RID: 30629
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040077A6 RID: 30630
		private InputField inputField;
	}
}
