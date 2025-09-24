using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EEC RID: 3820
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("End a centered GUILayout block started with {{GUILayoutBeginCentered}}.")]
	public class GUILayoutEndCentered : FsmStateAction
	{
		// Token: 0x06006B3D RID: 27453 RVA: 0x0021674F File Offset: 0x0021494F
		public override void Reset()
		{
		}

		// Token: 0x06006B3E RID: 27454 RVA: 0x00216751 File Offset: 0x00214951
		public override void OnGUI()
		{
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
	}
}
