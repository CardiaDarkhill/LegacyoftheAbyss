using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001178 RID: 4472
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the text value of a UI Text component.")]
	public class UiTextGetText : ComponentAction<Text>
	{
		// Token: 0x060077FF RID: 30719 RVA: 0x00246B52 File Offset: 0x00244D52
		public override void Reset()
		{
			this.text = null;
			this.everyFrame = false;
		}

		// Token: 0x06007800 RID: 30720 RVA: 0x00246B64 File Offset: 0x00244D64
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.uiText = this.cachedComponent;
			}
			this.DoGetTextValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007801 RID: 30721 RVA: 0x00246BAC File Offset: 0x00244DAC
		public override void OnUpdate()
		{
			this.DoGetTextValue();
		}

		// Token: 0x06007802 RID: 30722 RVA: 0x00246BB4 File Offset: 0x00244DB4
		private void DoGetTextValue()
		{
			if (this.uiText != null)
			{
				this.text.Value = this.uiText.text;
			}
		}

		// Token: 0x0400787F RID: 30847
		[RequiredField]
		[CheckForComponent(typeof(Text))]
		[Tooltip("The GameObject with the UI Text component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007880 RID: 30848
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The text value of the UI Text component.")]
		public FsmString text;

		// Token: 0x04007881 RID: 30849
		[Tooltip("Runs every frame. Useful to animate values over time.")]
		public bool everyFrame;

		// Token: 0x04007882 RID: 30850
		private Text uiText;
	}
}
