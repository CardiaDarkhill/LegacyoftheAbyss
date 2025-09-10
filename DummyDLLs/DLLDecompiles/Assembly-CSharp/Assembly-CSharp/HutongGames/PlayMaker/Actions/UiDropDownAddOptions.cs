using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200113B RID: 4411
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Add multiple options to the options of the Dropdown UI Component")]
	public class UiDropDownAddOptions : ComponentAction<Dropdown>
	{
		// Token: 0x060076CF RID: 30415 RVA: 0x002438EF File Offset: 0x00241AEF
		public override void Reset()
		{
			this.gameObject = null;
			this.optionText = new FsmString[1];
			this.optionImage = new FsmObject[1];
		}

		// Token: 0x060076D0 RID: 30416 RVA: 0x00243910 File Offset: 0x00241B10
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.dropDown = this.cachedComponent;
			}
			this.DoAddOptions();
			base.Finish();
		}

		// Token: 0x060076D1 RID: 30417 RVA: 0x00243950 File Offset: 0x00241B50
		private void DoAddOptions()
		{
			if (this.dropDown == null)
			{
				return;
			}
			this.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < this.optionText.Length; i++)
			{
				FsmString fsmString = this.optionText[i];
				this.options.Add(new Dropdown.OptionData
				{
					text = fsmString.Value,
					image = (this.optionImage[i].RawValue as Sprite)
				});
			}
			this.dropDown.AddOptions(this.options);
		}

		// Token: 0x04007741 RID: 30529
		[RequiredField]
		[CheckForComponent(typeof(Dropdown))]
		[Tooltip("The GameObject with the UI DropDown component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007742 RID: 30530
		[CompoundArray("Options", "Text", "Image")]
		[Tooltip("The text to use for this option.")]
		public FsmString[] optionText;

		// Token: 0x04007743 RID: 30531
		[ObjectType(typeof(Sprite))]
		[Tooltip("The image to use for this option.")]
		public FsmObject[] optionImage;

		// Token: 0x04007744 RID: 30532
		private Dropdown dropDown;

		// Token: 0x04007745 RID: 30533
		private List<Dropdown.OptionData> options;
	}
}
