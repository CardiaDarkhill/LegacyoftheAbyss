using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001159 RID: 4441
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the Asterix Character of a UI InputField component.")]
	public class UiInputFieldSetAsterixChar : ComponentAction<InputField>
	{
		// Token: 0x06007758 RID: 30552 RVA: 0x0024506D File Offset: 0x0024326D
		public override void Reset()
		{
			this.gameObject = null;
			this.asterixChar = "*";
			this.resetOnExit = null;
		}

		// Token: 0x06007759 RID: 30553 RVA: 0x00245090 File Offset: 0x00243290
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalValue = this.inputField.asteriskChar;
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x0600775A RID: 30554 RVA: 0x002450E4 File Offset: 0x002432E4
		private void DoSetValue()
		{
			char asteriskChar = UiInputFieldSetAsterixChar.__char__;
			if (this.asterixChar.Value.Length > 0)
			{
				asteriskChar = this.asterixChar.Value[0];
			}
			if (this.inputField != null)
			{
				this.inputField.asteriskChar = asteriskChar;
			}
		}

		// Token: 0x0600775B RID: 30555 RVA: 0x00245136 File Offset: 0x00243336
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.asteriskChar = this.originalValue;
			}
		}

		// Token: 0x040077D8 RID: 30680
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077D9 RID: 30681
		[RequiredField]
		[Tooltip("The asterix Character used for password field type of the UI InputField component. Only the first character will be used, the rest of the string will be ignored")]
		public FsmString asterixChar;

		// Token: 0x040077DA RID: 30682
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077DB RID: 30683
		private InputField inputField;

		// Token: 0x040077DC RID: 30684
		private char originalValue;

		// Token: 0x040077DD RID: 30685
		private static char __char__ = ' ';
	}
}
