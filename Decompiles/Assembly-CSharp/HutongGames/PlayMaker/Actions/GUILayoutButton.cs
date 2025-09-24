using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE8 RID: 3816
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
	public class GUILayoutButton : GUILayoutAction
	{
		// Token: 0x06006B31 RID: 27441 RVA: 0x0021648C File Offset: 0x0021468C
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

		// Token: 0x06006B32 RID: 27442 RVA: 0x002164E4 File Offset: 0x002146E4
		public override void OnGUI()
		{
			bool flag;
			if (string.IsNullOrEmpty(this.style.Value))
			{
				flag = GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				flag = GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			if (this.storeButtonState != null)
			{
				this.storeButtonState.Value = flag;
			}
		}

		// Token: 0x04006A83 RID: 27267
		[Tooltip("The Event to send when the button is pressed.")]
		public FsmEvent sendEvent;

		// Token: 0x04006A84 RID: 27268
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the button state in a bool variable.")]
		public FsmBool storeButtonState;

		// Token: 0x04006A85 RID: 27269
		[Tooltip("Texture to use on the button.")]
		public FsmTexture image;

		// Token: 0x04006A86 RID: 27270
		[Tooltip("Text to display on the button.")]
		public FsmString text;

		// Token: 0x04006A87 RID: 27271
		[Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006A88 RID: 27272
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
