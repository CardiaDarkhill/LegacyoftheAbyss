using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001150 RID: 4432
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the text value of a UI InputField component.")]
	public class UiInputFieldGetText : ComponentAction<InputField>
	{
		// Token: 0x0600772E RID: 30510 RVA: 0x00244942 File Offset: 0x00242B42
		public override void Reset()
		{
			this.text = null;
			this.everyFrame = false;
		}

		// Token: 0x0600772F RID: 30511 RVA: 0x00244954 File Offset: 0x00242B54
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.DoGetTextValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007730 RID: 30512 RVA: 0x0024499C File Offset: 0x00242B9C
		public override void OnUpdate()
		{
			this.DoGetTextValue();
		}

		// Token: 0x06007731 RID: 30513 RVA: 0x002449A4 File Offset: 0x00242BA4
		private void DoGetTextValue()
		{
			if (this.inputField != null)
			{
				this.text.Value = this.inputField.text;
			}
		}

		// Token: 0x040077A7 RID: 30631
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077A8 RID: 30632
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The text value of the UI InputField component.")]
		public FsmString text;

		// Token: 0x040077A9 RID: 30633
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040077AA RID: 30634
		private InputField inputField;
	}
}
