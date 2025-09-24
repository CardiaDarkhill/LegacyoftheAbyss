using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200115D RID: 4445
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the placeholder of a UI InputField component. Optionally reset on exit")]
	public class UiInputFieldSetPlaceHolder : ComponentAction<InputField>
	{
		// Token: 0x0600776F RID: 30575 RVA: 0x002453FA File Offset: 0x002435FA
		public override void Reset()
		{
			this.gameObject = null;
			this.placeholder = null;
			this.resetOnExit = null;
		}

		// Token: 0x06007770 RID: 30576 RVA: 0x00245414 File Offset: 0x00243614
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.originalValue = this.inputField.placeholder;
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x06007771 RID: 30577 RVA: 0x00245468 File Offset: 0x00243668
		private void DoSetValue()
		{
			if (this.inputField != null)
			{
				GameObject value = this.placeholder.Value;
				if (value == null)
				{
					this.inputField.placeholder = null;
					return;
				}
				this.inputField.placeholder = value.GetComponent<Graphic>();
			}
		}

		// Token: 0x06007772 RID: 30578 RVA: 0x002454B6 File Offset: 0x002436B6
		public override void OnExit()
		{
			if (this.inputField == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.inputField.placeholder = this.originalValue;
			}
		}

		// Token: 0x040077EF RID: 30703
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077F0 RID: 30704
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The placeholder (any graphic UI Component) for the UI InputField component.")]
		public FsmGameObject placeholder;

		// Token: 0x040077F1 RID: 30705
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040077F2 RID: 30706
		private InputField inputField;

		// Token: 0x040077F3 RID: 30707
		private Graphic originalValue;
	}
}
