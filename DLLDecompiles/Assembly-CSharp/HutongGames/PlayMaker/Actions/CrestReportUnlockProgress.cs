using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001323 RID: 4899
	public class CrestReportUnlockProgress : FsmStateAction
	{
		// Token: 0x06007EFC RID: 32508 RVA: 0x0025A51C File Offset: 0x0025871C
		public override void OnEnter()
		{
			Debug.LogError("DEPRECATED! Slot unlock XP system has been removed!");
			base.Finish();
		}
	}
}
