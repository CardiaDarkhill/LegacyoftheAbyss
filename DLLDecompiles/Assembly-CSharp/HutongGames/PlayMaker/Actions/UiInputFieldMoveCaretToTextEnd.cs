using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001154 RID: 4436
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Move Caret to text end in a UI InputField component. Optionally select from the current caret position")]
	public class UiInputFieldMoveCaretToTextEnd : ComponentAction<InputField>
	{
		// Token: 0x06007741 RID: 30529 RVA: 0x00244C7C File Offset: 0x00242E7C
		public override void Reset()
		{
			this.gameObject = null;
			this.shift = true;
		}

		// Token: 0x06007742 RID: 30530 RVA: 0x00244C94 File Offset: 0x00242E94
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

		// Token: 0x06007743 RID: 30531 RVA: 0x00244CD4 File Offset: 0x00242ED4
		private void DoAction()
		{
			if (this.inputField != null)
			{
				this.inputField.MoveTextEnd(this.shift.Value);
			}
		}

		// Token: 0x040077C2 RID: 30658
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077C3 RID: 30659
		[Tooltip("Define if we select or not from the current caret position. Default is true = no selection")]
		public FsmBool shift;

		// Token: 0x040077C4 RID: 30660
		private InputField inputField;
	}
}
