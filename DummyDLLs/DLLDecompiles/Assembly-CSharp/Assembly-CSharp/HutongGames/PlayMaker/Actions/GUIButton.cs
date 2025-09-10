using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ECB RID: 3787
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI button. Sends an Event when pressed. Optionally store the button state in a Bool Variable.")]
	public class GUIButton : GUIContentAction
	{
		// Token: 0x06006ADD RID: 27357 RVA: 0x00215390 File Offset: 0x00213590
		public override void Reset()
		{
			base.Reset();
			this.sendEvent = null;
			this.storeButtonState = null;
			this.style = "Button";
		}

		// Token: 0x06006ADE RID: 27358 RVA: 0x002153B8 File Offset: 0x002135B8
		public override void OnGUI()
		{
			base.OnGUI();
			bool value = false;
			if (GUI.Button(this.rect, this.content, this.style.Value))
			{
				base.Fsm.Event(this.sendEvent);
				value = true;
			}
			if (this.storeButtonState != null)
			{
				this.storeButtonState.Value = value;
			}
		}

		// Token: 0x04006A23 RID: 27171
		[Tooltip("The Event to send when the button is pressed.")]
		public FsmEvent sendEvent;

		// Token: 0x04006A24 RID: 27172
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the button state in a bool variable.")]
		public FsmBool storeButtonState;
	}
}
