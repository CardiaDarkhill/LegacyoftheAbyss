using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED9 RID: 3801
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the sorting depth of subsequent GUI elements.")]
	public class SetGUIDepth : FsmStateAction
	{
		// Token: 0x06006B07 RID: 27399 RVA: 0x00215B5C File Offset: 0x00213D5C
		public override void Reset()
		{
			this.depth = 0;
		}

		// Token: 0x06006B08 RID: 27400 RVA: 0x00215B6A File Offset: 0x00213D6A
		public override void OnPreprocess()
		{
			base.Fsm.HandleOnGUI = true;
		}

		// Token: 0x06006B09 RID: 27401 RVA: 0x00215B78 File Offset: 0x00213D78
		public override void OnGUI()
		{
			GUI.depth = this.depth.Value;
		}

		// Token: 0x04006A54 RID: 27220
		[RequiredField]
		[Tooltip("See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/GUI-depth.html\">GUI.Depth</a>.")]
		public FsmInt depth;
	}
}
