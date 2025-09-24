using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED7 RID: 3799
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the Tinting Color for the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIColor : FsmStateAction
	{
		// Token: 0x06006B01 RID: 27393 RVA: 0x00215AD6 File Offset: 0x00213CD6
		public override void Reset()
		{
			this.color = Color.white;
		}

		// Token: 0x06006B02 RID: 27394 RVA: 0x00215AE8 File Offset: 0x00213CE8
		public override void OnGUI()
		{
			GUI.color = this.color.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIColor = GUI.color;
			}
		}

		// Token: 0x04006A50 RID: 27216
		[RequiredField]
		[Tooltip("Tint Color for the GUI.")]
		public FsmColor color;

		// Token: 0x04006A51 RID: 27217
		[Tooltip("Apply this setting to all GUI calls, even in other scripts.")]
		public FsmBool applyGlobally;
	}
}
