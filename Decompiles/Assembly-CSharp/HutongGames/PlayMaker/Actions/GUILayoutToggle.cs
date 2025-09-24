using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EFC RID: 3836
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
	public class GUILayoutToggle : GUILayoutAction
	{
		// Token: 0x06006B6C RID: 27500 RVA: 0x00217014 File Offset: 0x00215214
		public override void Reset()
		{
			base.Reset();
			this.storeButtonState = null;
			this.text = "";
			this.image = null;
			this.tooltip = "";
			this.style = "Toggle";
			this.changedEvent = null;
		}

		// Token: 0x06006B6D RID: 27501 RVA: 0x0021706C File Offset: 0x0021526C
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.storeButtonState.Value = GUILayout.Toggle(this.storeButtonState.Value, new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006ABB RID: 27323
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Link the button state to this Bool Variable.")]
		public FsmBool storeButtonState;

		// Token: 0x04006ABC RID: 27324
		[Tooltip("Texture to display.")]
		public FsmTexture image;

		// Token: 0x04006ABD RID: 27325
		[Tooltip("Text to display.")]
		public FsmString text;

		// Token: 0x04006ABE RID: 27326
		[Tooltip("Optional tooltip. Accessed by {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006ABF RID: 27327
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		// Token: 0x04006AC0 RID: 27328
		[Tooltip("Optional Event to send when the toggle changes.")]
		public FsmEvent changedEvent;
	}
}
