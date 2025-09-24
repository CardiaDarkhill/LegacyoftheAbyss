using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200117C RID: 4476
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the isOn property of a UI Toggle component.")]
	public class UiToggleSetIsOn : ComponentAction<Toggle>
	{
		// Token: 0x06007814 RID: 30740 RVA: 0x00246EB2 File Offset: 0x002450B2
		public override void Reset()
		{
			this.gameObject = null;
			this.isOn = null;
			this.resetOnExit = null;
		}

		// Token: 0x06007815 RID: 30741 RVA: 0x00246ECC File Offset: 0x002450CC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this._toggle = this.cachedComponent;
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x06007816 RID: 30742 RVA: 0x00246F0C File Offset: 0x0024510C
		private void DoSetValue()
		{
			if (this._toggle != null)
			{
				this._originalValue = this._toggle.isOn;
				this._toggle.isOn = this.isOn.Value;
			}
		}

		// Token: 0x06007817 RID: 30743 RVA: 0x00246F43 File Offset: 0x00245143
		public override void OnExit()
		{
			if (this._toggle == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this._toggle.isOn = this._originalValue;
			}
		}

		// Token: 0x04007894 RID: 30868
		[RequiredField]
		[CheckForComponent(typeof(Toggle))]
		[Tooltip("The GameObject with the UI Toggle component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007895 RID: 30869
		[RequiredField]
		[Tooltip("Should the toggle be on?")]
		public FsmBool isOn;

		// Token: 0x04007896 RID: 30870
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007897 RID: 30871
		private Toggle _toggle;

		// Token: 0x04007898 RID: 30872
		private bool _originalValue;
	}
}
