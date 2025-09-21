using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED5 RID: 3797
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the global Alpha for the GUI. Useful for fading GUI up/down. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIAlpha : FsmStateAction
	{
		// Token: 0x06006AFB RID: 27387 RVA: 0x00215A1F File Offset: 0x00213C1F
		public override void Reset()
		{
			this.alpha = 1f;
		}

		// Token: 0x06006AFC RID: 27388 RVA: 0x00215A34 File Offset: 0x00213C34
		public override void OnGUI()
		{
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha.Value);
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIColor = GUI.color;
			}
		}

		// Token: 0x04006A4C RID: 27212
		[RequiredField]
		[Tooltip("Set the transparency of the GUI. 1 = opaque, 0 = transparent.")]
		public FsmFloat alpha;

		// Token: 0x04006A4D RID: 27213
		[Tooltip("Apply this setting to all GUI calls, even in other scripts.")]
		public FsmBool applyGlobally;
	}
}
