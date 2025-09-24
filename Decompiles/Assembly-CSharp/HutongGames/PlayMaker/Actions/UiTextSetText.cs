using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001179 RID: 4473
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the text value of a UI Text component.")]
	public class UiTextSetText : ComponentAction<Text>
	{
		// Token: 0x06007804 RID: 30724 RVA: 0x00246BE2 File Offset: 0x00244DE2
		public override void Reset()
		{
			this.gameObject = null;
			this.text = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x06007805 RID: 30725 RVA: 0x00246C00 File Offset: 0x00244E00
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.uiText = this.cachedComponent;
			}
			this.originalString = this.uiText.text;
			this.DoSetTextValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007806 RID: 30726 RVA: 0x00246C59 File Offset: 0x00244E59
		public override void OnUpdate()
		{
			this.DoSetTextValue();
		}

		// Token: 0x06007807 RID: 30727 RVA: 0x00246C61 File Offset: 0x00244E61
		private void DoSetTextValue()
		{
			if (this.uiText == null)
			{
				return;
			}
			this.uiText.text = this.text.Value;
		}

		// Token: 0x06007808 RID: 30728 RVA: 0x00246C88 File Offset: 0x00244E88
		public override void OnExit()
		{
			if (this.uiText == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.uiText.text = this.originalString;
			}
		}

		// Token: 0x04007883 RID: 30851
		[RequiredField]
		[CheckForComponent(typeof(Text))]
		[Tooltip("The GameObject with the UI Text component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007884 RID: 30852
		[UIHint(UIHint.TextArea)]
		[Tooltip("The text of the UI Text component.")]
		public FsmString text;

		// Token: 0x04007885 RID: 30853
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007886 RID: 30854
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007887 RID: 30855
		private Text uiText;

		// Token: 0x04007888 RID: 30856
		private string originalString;
	}
}
