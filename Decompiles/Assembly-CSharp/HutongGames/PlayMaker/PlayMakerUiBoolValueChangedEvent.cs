using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AE9 RID: 2793
	[AddComponentMenu("PlayMaker/UI/UI Bool Value Changed Event")]
	public class PlayMakerUiBoolValueChangedEvent : PlayMakerUiEventBase
	{
		// Token: 0x060058C6 RID: 22726 RVA: 0x001C3144 File Offset: 0x001C1344
		protected override void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			if (this.toggle == null)
			{
				this.toggle = base.GetComponent<Toggle>();
			}
			if (this.toggle != null)
			{
				this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
			}
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x001C31A5 File Offset: 0x001C13A5
		protected void OnDisable()
		{
			this.initialized = false;
			if (this.toggle != null)
			{
				this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
			}
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x001C31D8 File Offset: 0x001C13D8
		private void OnValueChanged(bool value)
		{
			Fsm.EventData.BoolData = value;
			base.SendEvent(FsmEvent.UiBoolValueChanged);
		}

		// Token: 0x04005414 RID: 21524
		public Toggle toggle;
	}
}
