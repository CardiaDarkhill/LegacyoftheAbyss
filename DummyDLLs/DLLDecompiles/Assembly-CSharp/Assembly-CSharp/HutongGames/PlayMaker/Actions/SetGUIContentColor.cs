using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED8 RID: 3800
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the Tinting Color for all text rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIContentColor : FsmStateAction
	{
		// Token: 0x06006B04 RID: 27396 RVA: 0x00215B19 File Offset: 0x00213D19
		public override void Reset()
		{
			this.contentColor = Color.white;
		}

		// Token: 0x06006B05 RID: 27397 RVA: 0x00215B2B File Offset: 0x00213D2B
		public override void OnGUI()
		{
			GUI.contentColor = this.contentColor.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIContentColor = GUI.contentColor;
			}
		}

		// Token: 0x04006A52 RID: 27218
		[RequiredField]
		[Tooltip("Tint color for all text rendered by the GUI.")]
		public FsmColor contentColor;

		// Token: 0x04006A53 RID: 27219
		[Tooltip("Apply this setting to all GUI calls, even in other scripts.")]
		public FsmBool applyGlobally;
	}
}
