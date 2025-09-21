using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EEF RID: 3823
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Close a group started with {{GUILayoutBeginVertical}}.")]
	public class GUILayoutEndVertical : FsmStateAction
	{
		// Token: 0x06006B45 RID: 27461 RVA: 0x00216794 File Offset: 0x00214994
		public override void Reset()
		{
		}

		// Token: 0x06006B46 RID: 27462 RVA: 0x00216796 File Offset: 0x00214996
		public override void OnGUI()
		{
			GUILayout.EndVertical();
		}
	}
}
