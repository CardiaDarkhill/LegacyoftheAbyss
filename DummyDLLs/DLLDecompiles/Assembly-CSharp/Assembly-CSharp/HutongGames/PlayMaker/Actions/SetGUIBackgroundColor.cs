using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED6 RID: 3798
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the Tinting Color for all background elements rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIBackgroundColor : FsmStateAction
	{
		// Token: 0x06006AFE RID: 27390 RVA: 0x00215A93 File Offset: 0x00213C93
		public override void Reset()
		{
			this.backgroundColor = Color.white;
		}

		// Token: 0x06006AFF RID: 27391 RVA: 0x00215AA5 File Offset: 0x00213CA5
		public override void OnGUI()
		{
			GUI.backgroundColor = this.backgroundColor.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIBackgroundColor = GUI.backgroundColor;
			}
		}

		// Token: 0x04006A4E RID: 27214
		[RequiredField]
		[Tooltip("The color for all background elements.")]
		public FsmColor backgroundColor;

		// Token: 0x04006A4F RID: 27215
		[Tooltip("Apply this setting to all GUI calls, even in other scripts.")]
		public FsmBool applyGlobally;
	}
}
