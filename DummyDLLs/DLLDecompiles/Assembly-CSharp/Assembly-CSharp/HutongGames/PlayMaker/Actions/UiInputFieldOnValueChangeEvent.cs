using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001158 RID: 4440
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Catches UI InputField onValueChanged event. Store the new value and/or send events. Event string data also contains the new value.")]
	public class UiInputFieldOnValueChangeEvent : ComponentAction<InputField>
	{
		// Token: 0x06007753 RID: 30547 RVA: 0x00244F91 File Offset: 0x00243191
		public override void Reset()
		{
			this.gameObject = null;
			this.text = null;
			this.eventTarget = null;
			this.sendEvent = null;
		}

		// Token: 0x06007754 RID: 30548 RVA: 0x00244FB0 File Offset: 0x002431B0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
				if (this.inputField != null)
				{
					this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.DoOnValueChange));
				}
			}
		}

		// Token: 0x06007755 RID: 30549 RVA: 0x0024500E File Offset: 0x0024320E
		public override void OnExit()
		{
			if (this.inputField != null)
			{
				this.inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.DoOnValueChange));
			}
		}

		// Token: 0x06007756 RID: 30550 RVA: 0x0024503A File Offset: 0x0024323A
		public void DoOnValueChange(string value)
		{
			this.text.Value = value;
			Fsm.EventData.StringData = value;
			base.SendEvent(this.eventTarget, this.sendEvent);
		}

		// Token: 0x040077D3 RID: 30675
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077D4 RID: 30676
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x040077D5 RID: 30677
		[Tooltip("Send this event when value changed.")]
		public FsmEvent sendEvent;

		// Token: 0x040077D6 RID: 30678
		[Tooltip("Store new value in string variable.")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		// Token: 0x040077D7 RID: 30679
		private InputField inputField;
	}
}
