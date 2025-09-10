using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200115A RID: 4442
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the caret's blink rate of a UI InputField component.")]
	public class UiInputFieldSetCaretBlinkRate : ComponentAction<InputField>
	{
		// Token: 0x0600775E RID: 30558 RVA: 0x00245176 File Offset: 0x00243376
		public override void Reset()
		{
			this.gameObject = null;
			this.caretBlinkRate = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x0600775F RID: 30559 RVA: 0x00245194 File Offset: 0x00243394
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalValue = this.inputField.caretBlinkRate;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007760 RID: 30560 RVA: 0x002451ED File Offset: 0x002433ED
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x06007761 RID: 30561 RVA: 0x002451F5 File Offset: 0x002433F5
		private void DoSetValue()
		{
			if (this.inputField != null)
			{
				this.inputField.caretBlinkRate = (float)this.caretBlinkRate.Value;
			}
		}

		// Token: 0x06007762 RID: 30562 RVA: 0x0024521C File Offset: 0x0024341C
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.caretBlinkRate = this.originalValue;
			}
		}

		// Token: 0x040077DE RID: 30686
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077DF RID: 30687
		[RequiredField]
		[Tooltip("The caret's blink rate for the UI InputField component.")]
		public FsmInt caretBlinkRate;

		// Token: 0x040077E0 RID: 30688
		[Tooltip("Deactivate when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077E1 RID: 30689
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040077E2 RID: 30690
		private InputField inputField;

		// Token: 0x040077E3 RID: 30691
		private float originalValue;
	}
}
