using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001153 RID: 4435
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the cancel state of a UI InputField component. This relates to the last onEndEdit Event")]
	public class UiInputFieldGetWasCanceled : ComponentAction<InputField>
	{
		// Token: 0x0600773D RID: 30525 RVA: 0x00244BC3 File Offset: 0x00242DC3
		public override void Reset()
		{
			this.wasCanceled = null;
			this.wasCanceledEvent = null;
			this.wasNotCanceledEvent = null;
		}

		// Token: 0x0600773E RID: 30526 RVA: 0x00244BDC File Offset: 0x00242DDC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x0600773F RID: 30527 RVA: 0x00244C1C File Offset: 0x00242E1C
		private void DoGetValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			this.wasCanceled.Value = this.inputField.wasCanceled;
			base.Fsm.Event(this.inputField.wasCanceled ? this.wasCanceledEvent : this.wasNotCanceledEvent);
		}

		// Token: 0x040077BD RID: 30653
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077BE RID: 30654
		[UIHint(UIHint.Variable)]
		[Tooltip("The was canceled flag value of the UI InputField component.")]
		public FsmBool wasCanceled;

		// Token: 0x040077BF RID: 30655
		[Tooltip("Event sent if inputField was canceled")]
		public FsmEvent wasCanceledEvent;

		// Token: 0x040077C0 RID: 30656
		[Tooltip("Event sent if inputField was not canceled")]
		public FsmEvent wasNotCanceledEvent;

		// Token: 0x040077C1 RID: 30657
		private InputField inputField;
	}
}
