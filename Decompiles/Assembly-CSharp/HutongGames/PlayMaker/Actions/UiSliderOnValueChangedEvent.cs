using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001172 RID: 4466
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Catches onValueChanged event for a UI Slider component. Store the new value and/or send events. Event float data will contain the new slider value")]
	public class UiSliderOnValueChangedEvent : ComponentAction<Slider>
	{
		// Token: 0x060077DE RID: 30686 RVA: 0x00246535 File Offset: 0x00244735
		public override void Reset()
		{
			this.gameObject = null;
			this.eventTarget = null;
			this.sendEvent = null;
			this.value = null;
		}

		// Token: 0x060077DF RID: 30687 RVA: 0x00246554 File Offset: 0x00244754
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
				if (this.slider != null)
				{
					this.slider.onValueChanged.AddListener(new UnityAction<float>(this.DoOnValueChanged));
				}
			}
		}

		// Token: 0x060077E0 RID: 30688 RVA: 0x002465B2 File Offset: 0x002447B2
		public override void OnExit()
		{
			if (this.slider != null)
			{
				this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.DoOnValueChanged));
			}
		}

		// Token: 0x060077E1 RID: 30689 RVA: 0x002465DE File Offset: 0x002447DE
		public void DoOnValueChanged(float _value)
		{
			this.value.Value = _value;
			Fsm.EventData.FloatData = _value;
			base.SendEvent(this.eventTarget, this.sendEvent);
		}

		// Token: 0x0400785B RID: 30811
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400785C RID: 30812
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x0400785D RID: 30813
		[Tooltip("Send this event when Clicked.")]
		public FsmEvent sendEvent;

		// Token: 0x0400785E RID: 30814
		[Tooltip("Store the new value in float variable.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat value;

		// Token: 0x0400785F RID: 30815
		private Slider slider;
	}
}
