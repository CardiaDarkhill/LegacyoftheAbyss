using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF0 RID: 3824
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Inserts a flexible space element.")]
	public class GUILayoutFlexibleSpace : FsmStateAction
	{
		// Token: 0x06006B48 RID: 27464 RVA: 0x002167A5 File Offset: 0x002149A5
		public override void Reset()
		{
		}

		// Token: 0x06006B49 RID: 27465 RVA: 0x002167A7 File Offset: 0x002149A7
		public override void OnGUI()
		{
			GUILayout.FlexibleSpace();
		}
	}
}
