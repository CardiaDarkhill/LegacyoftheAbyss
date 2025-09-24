using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF9 RID: 3833
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Inserts a space in the current layout group.")]
	public class GUILayoutSpace : FsmStateAction
	{
		// Token: 0x06006B63 RID: 27491 RVA: 0x00216E99 File Offset: 0x00215099
		public override void Reset()
		{
			this.space = 10f;
		}

		// Token: 0x06006B64 RID: 27492 RVA: 0x00216EAB File Offset: 0x002150AB
		public override void OnGUI()
		{
			GUILayout.Space(this.space.Value);
		}

		// Token: 0x04006AB4 RID: 27316
		[Tooltip("Size of space in pixels.")]
		public FsmFloat space;
	}
}
