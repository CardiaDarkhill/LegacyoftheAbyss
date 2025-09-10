using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200115B RID: 4443
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the maximum number of characters that the user can type into a UI InputField component. Optionally reset on exit")]
	public class UiInputFieldSetCharacterLimit : ComponentAction<InputField>
	{
		// Token: 0x06007764 RID: 30564 RVA: 0x00245253 File Offset: 0x00243453
		public override void Reset()
		{
			this.gameObject = null;
			this.characterLimit = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x06007765 RID: 30565 RVA: 0x00245274 File Offset: 0x00243474
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalValue = this.inputField.characterLimit;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007766 RID: 30566 RVA: 0x002452CD File Offset: 0x002434CD
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x06007767 RID: 30567 RVA: 0x002452D5 File Offset: 0x002434D5
		private void DoSetValue()
		{
			if (this.inputField != null)
			{
				this.inputField.characterLimit = this.characterLimit.Value;
			}
		}

		// Token: 0x06007768 RID: 30568 RVA: 0x002452FB File Offset: 0x002434FB
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.characterLimit = this.originalValue;
			}
		}

		// Token: 0x040077E4 RID: 30692
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077E5 RID: 30693
		[RequiredField]
		[Tooltip("The maximum number of characters that the user can type into the UI InputField component. 0 = infinite")]
		public FsmInt characterLimit;

		// Token: 0x040077E6 RID: 30694
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077E7 RID: 30695
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040077E8 RID: 30696
		private InputField inputField;

		// Token: 0x040077E9 RID: 30697
		private int originalValue;
	}
}
