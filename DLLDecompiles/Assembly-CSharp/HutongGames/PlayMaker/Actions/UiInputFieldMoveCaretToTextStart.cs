using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001155 RID: 4437
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Move Caret to text start in a UI InputField component. Optionally select from the current caret position")]
	public class UiInputFieldMoveCaretToTextStart : ComponentAction<InputField>
	{
		// Token: 0x06007745 RID: 30533 RVA: 0x00244D02 File Offset: 0x00242F02
		public override void Reset()
		{
			this.gameObject = null;
			this.shift = true;
		}

		// Token: 0x06007746 RID: 30534 RVA: 0x00244D18 File Offset: 0x00242F18
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

		// Token: 0x06007747 RID: 30535 RVA: 0x00244D58 File Offset: 0x00242F58
		private void DoAction()
		{
			if (this.inputField != null)
			{
				this.inputField.MoveTextStart(this.shift.Value);
			}
		}

		// Token: 0x040077C5 RID: 30661
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077C6 RID: 30662
		[Tooltip("Define if we select or not from the current caret position. Default is true = no selection")]
		public FsmBool shift;

		// Token: 0x040077C7 RID: 30663
		private InputField inputField;
	}
}
