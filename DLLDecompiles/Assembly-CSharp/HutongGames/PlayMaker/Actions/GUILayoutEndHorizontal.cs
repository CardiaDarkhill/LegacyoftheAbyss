using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EED RID: 3821
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Close a group started with {{GUILayoutBeginHorizontal}}.")]
	public class GUILayoutEndHorizontal : FsmStateAction
	{
		// Token: 0x06006B40 RID: 27456 RVA: 0x00216774 File Offset: 0x00214974
		public override void Reset()
		{
		}

		// Token: 0x06006B41 RID: 27457 RVA: 0x00216776 File Offset: 0x00214976
		public override void OnGUI()
		{
			GUILayout.EndHorizontal();
		}
	}
}
