using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001156 RID: 4438
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Fires an event when editing ended in a UI InputField component. Event string data will contain the text value, and the boolean will be true is it was a cancel action")]
	public class UiInputFieldOnEndEditEvent : ComponentAction<InputField>
	{
		// Token: 0x06007749 RID: 30537 RVA: 0x00244D86 File Offset: 0x00242F86
		public override void Reset()
		{
			this.gameObject = null;
			this.sendEvent = null;
			this.text = null;
			this.wasCanceled = null;
		}

		// Token: 0x0600774A RID: 30538 RVA: 0x00244DA4 File Offset: 0x00242FA4
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

		// Token: 0x0600774B RID: 30539 RVA: 0x00244E02 File Offset: 0x00243002
		public override void OnExit()
		{
			if (this.inputField != null)
			{
				this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.DoOnEndEdit));
			}
		}

		// Token: 0x0600774C RID: 30540 RVA: 0x00244E30 File Offset: 0x00243030
		public void DoOnEndEdit(string value)
		{
			this.text.Value = value;
			this.wasCanceled.Value = this.inputField.wasCanceled;
			Fsm.EventData.StringData = value;
			Fsm.EventData.BoolData = this.inputField.wasCanceled;
			base.SendEvent(this.eventTarget, this.sendEvent);
			base.Finish();
		}

		// Token: 0x040077C8 RID: 30664
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077C9 RID: 30665
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x040077CA RID: 30666
		[Tooltip("Send this event when editing ended.")]
		public FsmEvent sendEvent;

		// Token: 0x040077CB RID: 30667
		[Tooltip("The content of the InputField when edited ended")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		// Token: 0x040077CC RID: 30668
		[Tooltip("The canceled state of the InputField when edited ended")]
		[UIHint(UIHint.Variable)]
		public FsmBool wasCanceled;

		// Token: 0x040077CD RID: 30669
		private InputField inputField;
	}
}
