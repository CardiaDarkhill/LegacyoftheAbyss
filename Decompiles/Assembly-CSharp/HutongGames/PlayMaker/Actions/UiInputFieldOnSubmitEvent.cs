using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001157 RID: 4439
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Fires an event when user submits from a UI InputField component. \nThis only fires if the user press Enter, not when field looses focus or user escaped the field.\nEvent string data will contain the text value.")]
	public class UiInputFieldOnSubmitEvent : ComponentAction<InputField>
	{
		// Token: 0x0600774E RID: 30542 RVA: 0x00244E9F File Offset: 0x0024309F
		public override void Reset()
		{
			this.gameObject = null;
			this.eventTarget = null;
			this.sendEvent = null;
			this.text = null;
		}

		// Token: 0x0600774F RID: 30543 RVA: 0x00244EC0 File Offset: 0x002430C0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
				if (this.inputField != null)
				{
					this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.DoOnEndEdit));
				}
			}
		}

		// Token: 0x06007750 RID: 30544 RVA: 0x00244F1E File Offset: 0x0024311E
		public override void OnExit()
		{
			if (this.inputField != null)
			{
				this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.DoOnEndEdit));
			}
		}

		// Token: 0x06007751 RID: 30545 RVA: 0x00244F4A File Offset: 0x0024314A
		public void DoOnEndEdit(string value)
		{
			if (this.inputField.wasCanceled)
			{
				return;
			}
			this.text.Value = value;
			Fsm.EventData.StringData = value;
			base.SendEvent(this.eventTarget, this.sendEvent);
			base.Finish();
		}

		// Token: 0x040077CE RID: 30670
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077CF RID: 30671
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x040077D0 RID: 30672
		[Tooltip("Send this event when editing ended.")]
		public FsmEvent sendEvent;

		// Token: 0x040077D1 RID: 30673
		[Tooltip("The content of the InputField when submitting")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		// Token: 0x040077D2 RID: 30674
		private InputField inputField;
	}
}
