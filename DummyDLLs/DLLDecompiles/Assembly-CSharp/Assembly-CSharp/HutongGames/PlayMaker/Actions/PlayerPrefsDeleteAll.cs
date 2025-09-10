using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FEE RID: 4078
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Removes all keys and values from the preferences. Use with caution.")]
	public class PlayerPrefsDeleteAll : FsmStateAction
	{
		// Token: 0x06007068 RID: 28776 RVA: 0x0022B9B0 File Offset: 0x00229BB0
		public override void Reset()
		{
		}

		// Token: 0x06007069 RID: 28777 RVA: 0x0022B9B2 File Offset: 0x00229BB2
		public override void OnEnter()
		{
			PlayerPrefs.DeleteAll();
			base.Finish();
		}
	}
}
