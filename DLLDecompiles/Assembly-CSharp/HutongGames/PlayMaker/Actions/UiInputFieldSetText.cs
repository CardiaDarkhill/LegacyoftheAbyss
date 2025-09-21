using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200115F RID: 4447
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the text value of a UI InputField component.")]
	public class UiInputFieldSetText : ComponentAction<InputField>
	{
		// Token: 0x0600777A RID: 30586 RVA: 0x002455CA File Offset: 0x002437CA
		public override void Reset()
		{
			this.gameObject = null;
			this.text = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x0600777B RID: 30587 RVA: 0x002455E8 File Offset: 0x002437E8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalString = this.inputField.text;
			this.DoSetTextValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600777C RID: 30588 RVA: 0x00245641 File Offset: 0x00243841
		public override void OnUpdate()
		{
			this.DoSetTextValue();
		}

		// Token: 0x0600777D RID: 30589 RVA: 0x00245649 File Offset: 0x00243849
		private void DoSetTextValue()
		{
			if (this.inputField != null)
			{
				this.inputField.text = this.text.Value;
			}
		}

		// Token: 0x0600777E RID: 30590 RVA: 0x0024566F File Offset: 0x0024386F
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.text = this.originalString;
			}
		}

		// Token: 0x040077FA RID: 30714
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077FB RID: 30715
		[UIHint(UIHint.TextArea)]
		[Tooltip("The text of the UI InputField component.")]
		public FsmString text;

		// Token: 0x040077FC RID: 30716
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077FD RID: 30717
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040077FE RID: 30718
		private InputField inputField;

		// Token: 0x040077FF RID: 30719
		private string originalString;
	}
}
