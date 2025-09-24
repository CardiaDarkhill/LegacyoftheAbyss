using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AED RID: 2797
	[AddComponentMenu("PlayMaker/UI/UI End Edit Event")]
	public class PlayMakerUiEndEditEvent : PlayMakerUiEventBase
	{
		// Token: 0x060058D4 RID: 22740 RVA: 0x001C3300 File Offset: 0x001C1500
		protected override void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			if (this.inputField == null)
			{
				this.inputField = base.GetComponent<InputField>();
			}
			if (this.inputField != null)
			{
				this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.DoOnEndEdit));
			}
		}

		// Token: 0x060058D5 RID: 22741 RVA: 0x001C3361 File Offset: 0x001C1561
		protected void OnDisable()
		{
			this.initialized = false;
			if (this.inputField != null)
			{
				this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.DoOnEndEdit));
			}
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x001C3394 File Offset: 0x001C1594
		private void DoOnEndEdit(string value)
		{
			Fsm.EventData.StringData = value;
			base.SendEvent(FsmEvent.UiEndEdit);
		}

		// Token: 0x04005416 RID: 21526
		public InputField inputField;
	}
}
