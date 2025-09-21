using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF8 RID: 3832
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Repeat Button. Sends an Event while pressed. Optionally store the button state in a Bool Variable.")]
	public class GUILayoutRepeatButton : GUILayoutAction
	{
		// Token: 0x06006B60 RID: 27488 RVA: 0x00216D84 File Offset: 0x00214F84
		public override void Reset()
		{
			base.Reset();
			this.sendEvent = null;
			this.storeButtonState = null;
			this.text = "";
			this.image = null;
			this.tooltip = "";
			this.style = "";
		}

		// Token: 0x06006B61 RID: 27489 RVA: 0x00216DDC File Offset: 0x00214FDC
		public override void OnGUI()
		{
			bool flag;
			if (string.IsNullOrEmpty(this.style.Value))
			{
				flag = GUILayout.RepeatButton(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				flag = GUILayout.RepeatButton(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeButtonState.Value = flag;
		}

		// Token: 0x04006AAE RID: 27310
		[Tooltip("The fsm event to send while the button is pressed.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AAF RID: 27311
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the button state in a Bool Variable.")]
		public FsmBool storeButtonState;

		// Token: 0x04006AB0 RID: 27312
		[Tooltip("The texture to display.")]
		public FsmTexture image;

		// Token: 0x04006AB1 RID: 27313
		[Tooltip("The text to display.")]
		public FsmString text;

		// Token: 0x04006AB2 RID: 27314
		[Tooltip("Optional tooltip. Accessed by {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006AB3 RID: 27315
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
