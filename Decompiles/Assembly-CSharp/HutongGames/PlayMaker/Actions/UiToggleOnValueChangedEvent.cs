using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200117B RID: 4475
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Catches onValueChanged event in a UI Toggle component. Store the new value and/or send events. Event bool data will contain the new Toggle value")]
	public class UiToggleOnValueChangedEvent : ComponentAction<Toggle>
	{
		// Token: 0x0600780F RID: 30735 RVA: 0x00246D88 File Offset: 0x00244F88
		public override void Reset()
		{
			this.gameObject = null;
			this.eventTarget = null;
			this.sendEvent = null;
			this.value = null;
		}

		// Token: 0x06007810 RID: 30736 RVA: 0x00246DA8 File Offset: 0x00244FA8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.LogError("Missing GameObject");
				return;
			}
			if (this.toggle != null)
			{
				this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.DoOnValueChanged));
			}
			this.toggle = this.cachedComponent;
			if (this.toggle != null)
			{
				this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.DoOnValueChanged));
				return;
			}
			base.LogError("Missing UI.Toggle on " + ownerDefaultTarget.name);
		}

		// Token: 0x06007811 RID: 30737 RVA: 0x00246E53 File Offset: 0x00245053
		public override void OnExit()
		{
			if (this.toggle != null)
			{
				this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.DoOnValueChanged));
			}
		}

		// Token: 0x06007812 RID: 30738 RVA: 0x00246E7F File Offset: 0x0024507F
		public void DoOnValueChanged(bool _value)
		{
			this.value.Value = _value;
			Fsm.EventData.BoolData = _value;
			base.SendEvent(this.eventTarget, this.sendEvent);
		}

		// Token: 0x0400788F RID: 30863
		[RequiredField]
		[CheckForComponent(typeof(Toggle))]
		[Tooltip("The GameObject with the UI Toggle component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007890 RID: 30864
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04007891 RID: 30865
		[Tooltip("Send this event when the value changes.")]
		public FsmEvent sendEvent;

		// Token: 0x04007892 RID: 30866
		[Tooltip("Store the new value in bool variable.")]
		[UIHint(UIHint.Variable)]
		public FsmBool value;

		// Token: 0x04007893 RID: 30867
		private Toggle toggle;
	}
}
