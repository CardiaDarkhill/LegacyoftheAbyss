using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200114B RID: 4427
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the Character Limit value of a UI InputField component. This is the maximum number of characters that the user can type into the field.")]
	public class UiInputFieldGetCharacterLimit : ComponentAction<InputField>
	{
		// Token: 0x06007718 RID: 30488 RVA: 0x002445A2 File Offset: 0x002427A2
		public override void Reset()
		{
			this.characterLimit = null;
			this.everyFrame = false;
		}

		// Token: 0x06007719 RID: 30489 RVA: 0x002445B4 File Offset: 0x002427B4
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

		// Token: 0x0600771A RID: 30490 RVA: 0x002445FC File Offset: 0x002427FC
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x0600771B RID: 30491 RVA: 0x00244604 File Offset: 0x00242804
		private void DoGetValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			this.characterLimit.Value = this.inputField.characterLimit;
			base.Fsm.Event((this.inputField.characterLimit > 0) ? this.isLimitedEvent : this.hasNoLimitEvent);
		}

		// Token: 0x0400778D RID: 30605
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400778E RID: 30606
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The maximum number of characters that the user can type into the UI InputField component.")]
		public FsmInt characterLimit;

		// Token: 0x0400778F RID: 30607
		[Tooltip("Event sent if limit is infinite (equal to 0)")]
		public FsmEvent hasNoLimitEvent;

		// Token: 0x04007790 RID: 30608
		[Tooltip("Event sent if limit is more than 0")]
		public FsmEvent isLimitedEvent;

		// Token: 0x04007791 RID: 30609
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x04007792 RID: 30610
		private InputField inputField;
	}
}
