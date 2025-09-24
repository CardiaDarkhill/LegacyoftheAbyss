using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200113E RID: 4414
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Set the selected value (zero based index) of the UI Dropdown Component")]
	public class UiDropDownSetValue : ComponentAction<Dropdown>
	{
		// Token: 0x060076DB RID: 30427 RVA: 0x00243B73 File Offset: 0x00241D73
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.everyFrame = false;
		}

		// Token: 0x060076DC RID: 30428 RVA: 0x00243B8C File Offset: 0x00241D8C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.dropDown = this.cachedComponent;
			}
			this.SetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076DD RID: 30429 RVA: 0x00243BD4 File Offset: 0x00241DD4
		public override void OnUpdate()
		{
			this.SetValue();
		}

		// Token: 0x060076DE RID: 30430 RVA: 0x00243BDC File Offset: 0x00241DDC
		private void SetValue()
		{
			if (this.dropDown == null)
			{
				return;
			}
			if (this.dropDown.value != this.value.Value)
			{
				this.dropDown.value = this.value.Value;
			}
		}

		// Token: 0x0400774E RID: 30542
		[RequiredField]
		[CheckForComponent(typeof(Dropdown))]
		[Tooltip("The GameObject with the UI DropDown component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400774F RID: 30543
		[RequiredField]
		[Tooltip("The selected index of the dropdown (zero based index).")]
		public FsmInt value;

		// Token: 0x04007750 RID: 30544
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007751 RID: 30545
		private Dropdown dropDown;
	}
}
