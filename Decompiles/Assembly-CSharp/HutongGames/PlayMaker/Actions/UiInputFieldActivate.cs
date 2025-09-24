using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001148 RID: 4424
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Activate a UI InputField component to begin processing Events. Optionally Deactivate on state exit")]
	public class UiInputFieldActivate : ComponentAction<InputField>
	{
		// Token: 0x06007709 RID: 30473 RVA: 0x002443DC File Offset: 0x002425DC
		public override void Reset()
		{
			this.gameObject = null;
			this.deactivateOnExit = null;
		}

		// Token: 0x0600770A RID: 30474 RVA: 0x002443EC File Offset: 0x002425EC
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

		// Token: 0x0600770B RID: 30475 RVA: 0x0024442C File Offset: 0x0024262C
		private void DoAction()
		{
			if (this.inputField != null)
			{
				this.inputField.ActivateInputField();
			}
		}

		// Token: 0x0600770C RID: 30476 RVA: 0x00244447 File Offset: 0x00242647
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.deactivateOnExit.Value)
			{
				this.inputField.DeactivateInputField();
			}
		}

		// Token: 0x04007783 RID: 30595
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007784 RID: 30596
		[Tooltip("Reset when exiting this state.")]
		public FsmBool deactivateOnExit;

		// Token: 0x04007785 RID: 30597
		private InputField inputField;
	}
}
