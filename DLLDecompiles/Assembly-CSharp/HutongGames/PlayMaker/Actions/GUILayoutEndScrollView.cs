using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EEE RID: 3822
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Close a group started with {{GUILayoutBeginScrollView}}.")]
	public class GUILayoutEndScrollView : FsmStateAction
	{
		// Token: 0x06006B43 RID: 27459 RVA: 0x00216785 File Offset: 0x00214985
		public override void OnGUI()
		{
			GUILayout.EndScrollView();
		}
	}
}
