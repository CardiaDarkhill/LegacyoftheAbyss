using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001149 RID: 4425
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Deactivate a UI InputField to stop the processing of Events and send OnSubmit if not canceled. Optionally Activate on state exit")]
	public class UiInputFieldDeactivate : ComponentAction<InputField>
	{
		// Token: 0x0600770E RID: 30478 RVA: 0x00244478 File Offset: 0x00242678
		public override void Reset()
		{
			this.gameObject = null;
			this.activateOnExit = null;
		}

		// Token: 0x0600770F RID: 30479 RVA: 0x00244488 File Offset: 0x00242688
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.DoAction();
			base.Finish();
		}

		// Token: 0x06007710 RID: 30480 RVA: 0x002444C8 File Offset: 0x002426C8
		private void DoAction()
		{
			if (this.inputField != null)
			{
				this.inputField.DeactivateInputField();
			}
		}

		// Token: 0x06007711 RID: 30481 RVA: 0x002444E3 File Offset: 0x002426E3
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.activateOnExit.Value)
			{
				this.inputField.ActivateInputField();
			}
		}

		// Token: 0x04007786 RID: 30598
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007787 RID: 30599
		[Tooltip("Activate when exiting this state.")]
		public FsmBool activateOnExit;

		// Token: 0x04007788 RID: 30600
		private InputField inputField;
	}
}
