using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001165 RID: 4453
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Catches UI Scrollbar onValueChanged event. Store the new value and/or send events. Event float data will contain the new Scrollbar value")]
	public class UiScrollbarOnValueChanged : ComponentAction<Scrollbar>
	{
		// Token: 0x06007799 RID: 30617 RVA: 0x002459DA File Offset: 0x00243BDA
		public override void Reset()
		{
			this.gameObject = null;
			this.eventTarget = null;
			this.sendEvent = null;
			this.value = null;
		}

		// Token: 0x0600779A RID: 30618 RVA: 0x002459F8 File Offset: 0x00243BF8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollbar = this.cachedComponent;
				if (this.scrollbar != null)
				{
					this.scrollbar.onValueChanged.AddListener(new UnityAction<float>(this.DoOnValueChanged));
				}
			}
		}

		// Token: 0x0600779B RID: 30619 RVA: 0x00245A56 File Offset: 0x00243C56
		public override void OnExit()
		{
			if (this.scrollbar != null)
			{
				this.scrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.DoOnValueChanged));
			}
		}

		// Token: 0x0600779C RID: 30620 RVA: 0x00245A82 File Offset: 0x00243C82
		public void DoOnValueChanged(float _value)
		{
			this.value.Value = _value;
			Fsm.EventData.FloatData = _value;
			base.SendEvent(this.eventTarget, this.sendEvent);
		}

		// Token: 0x04007815 RID: 30741
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007816 RID: 30742
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04007817 RID: 30743
		[Tooltip("Send this event when the UI Scrollbar value changes.")]
		public FsmEvent sendEvent;

		// Token: 0x04007818 RID: 30744
		[Tooltip("Store new value in float variable.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat value;

		// Token: 0x04007819 RID: 30745
		private Scrollbar scrollbar;
	}
}
