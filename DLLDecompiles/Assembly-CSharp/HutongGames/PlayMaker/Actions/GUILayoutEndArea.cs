using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EEB RID: 3819
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Close a GUILayout group started with {{GUILayoutBeginArea}}.")]
	public class GUILayoutEndArea : FsmStateAction
	{
		// Token: 0x06006B3A RID: 27450 RVA: 0x0021673E File Offset: 0x0021493E
		public override void Reset()
		{
		}

		// Token: 0x06006B3B RID: 27451 RVA: 0x00216740 File Offset: 0x00214940
		public override void OnGUI()
		{
			GUILayout.EndArea();
		}
	}
}
