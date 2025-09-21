using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED2 RID: 3794
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Resets the GUI matrix. Useful if you've rotated or scaled the GUI and now want to reset it.")]
	public class ResetGUIMatrix : FsmStateAction
	{
		// Token: 0x06006AF1 RID: 27377 RVA: 0x002157DD File Offset: 0x002139DD
		public override void OnGUI()
		{
			PlayMakerGUI.GUIMatrix = (GUI.matrix = Matrix4x4.identity);
		}
	}
}
