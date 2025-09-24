using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200113D RID: 4413
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Get the selected value (zero based index), sprite and text from a UI Dropdown Component")]
	public class UiDropDownGetSelectedData : ComponentAction<Dropdown>
	{
		// Token: 0x060076D6 RID: 30422 RVA: 0x00243A47 File Offset: 0x00241C47
		public override void Reset()
		{
			this.gameObject = null;
			this.index = null;
			this.getText = null;
			this.getImage = null;
			this.everyFrame = false;
		}

		// Token: 0x060076D7 RID: 30423 RVA: 0x00243A6C File Offset: 0x00241C6C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.dropDown = this.cachedComponent;
			}
			this.GetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076D8 RID: 30424 RVA: 0x00243AB4 File Offset: 0x00241CB4
		public override void OnUpdate()
		{
			this.GetValue();
		}

		// Token: 0x060076D9 RID: 30425 RVA: 0x00243ABC File Offset: 0x00241CBC
		private void GetValue()
		{
			if (this.dropDown == null)
			{
				return;
			}
			if (!this.index.IsNone)
			{
				this.index.Value = this.dropDown.value;
			}
			if (!this.getText.IsNone)
			{
				this.getText.Value = this.dropDown.options[this.dropDown.value].text;
			}
			if (!this.getImage.IsNone)
			{
				this.getImage.Value = this.dropDown.options[this.dropDown.value].image;
			}
		}

		// Token: 0x04007748 RID: 30536
		[RequiredField]
		[CheckForComponent(typeof(Dropdown))]
		[Tooltip("The GameObject with the UI DropDown component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007749 RID: 30537
		[Tooltip("The selected index of the dropdown (zero based index).")]
		[UIHint(UIHint.Variable)]
		public FsmInt index;

		// Token: 0x0400774A RID: 30538
		[Tooltip("The selected text.")]
		[UIHint(UIHint.Variable)]
		public FsmString getText;

		// Token: 0x0400774B RID: 30539
		[ObjectType(typeof(Sprite))]
		[Tooltip("The selected text.")]
		[UIHint(UIHint.Variable)]
		public FsmObject getImage;

		// Token: 0x0400774C RID: 30540
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x0400774D RID: 30541
		private Dropdown dropDown;
	}
}
