using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE3 RID: 3811
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begin a centered GUILayout block. The block is centered inside a parent {{GUILayout Area}}. So to place the block in the center of the screen, first use a {{GULayout Area}} the size of the whole screen (the default setting). NOTE: Block must end with a corresponding {{GUILayoutEndCentered}}.")]
	public class GUILayoutBeginCentered : FsmStateAction
	{
		// Token: 0x06006B22 RID: 27426 RVA: 0x00216153 File Offset: 0x00214353
		public override void Reset()
		{
		}

		// Token: 0x06006B23 RID: 27427 RVA: 0x00216155 File Offset: 0x00214355
		public override void OnGUI()
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		}
	}
}
