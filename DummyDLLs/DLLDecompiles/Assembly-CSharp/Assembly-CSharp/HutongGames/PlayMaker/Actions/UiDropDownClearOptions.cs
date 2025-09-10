using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200113C RID: 4412
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Clear the list of options in a UI Dropdown Component")]
	public class UiDropDownClearOptions : ComponentAction<Dropdown>
	{
		// Token: 0x060076D3 RID: 30419 RVA: 0x002439E0 File Offset: 0x00241BE0
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060076D4 RID: 30420 RVA: 0x002439EC File Offset: 0x00241BEC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.dropDown = this.cachedComponent;
			}
			if (this.dropDown != null)
			{
				this.dropDown.ClearOptions();
			}
			base.Finish();
		}

		// Token: 0x04007746 RID: 30534
		[RequiredField]
		[CheckForComponent(typeof(Dropdown))]
		[Tooltip("The GameObject with the UI DropDown component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007747 RID: 30535
		private Dropdown dropDown;
	}
}
