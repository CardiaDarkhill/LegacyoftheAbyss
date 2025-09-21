using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED0 RID: 3792
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Gets the Tooltip of the control the mouse is currently over and store it in a String Variable.")]
	public class GUITooltip : FsmStateAction
	{
		// Token: 0x06006AEB RID: 27371 RVA: 0x00215694 File Offset: 0x00213894
		public override void Reset()
		{
			this.storeTooltip = null;
		}

		// Token: 0x06006AEC RID: 27372 RVA: 0x0021569D File Offset: 0x0021389D
		public override void OnGUI()
		{
			this.storeTooltip.Value = GUI.tooltip;
		}

		// Token: 0x04006A39 RID: 27193
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the tooltip in a string variable.")]
		public FsmString storeTooltip;
	}
}
