using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEC RID: 3052
	[ActionCategory(ActionCategory.Application)]
	public class CheckIsInEditor : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x06005D79 RID: 23929 RVA: 0x001D77E4 File Offset: 0x001D59E4
		public override bool IsTrue
		{
			get
			{
				return Application.isEditor;
			}
		}
	}
}
