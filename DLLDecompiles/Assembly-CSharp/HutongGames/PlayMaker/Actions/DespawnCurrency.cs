using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012EB RID: 4843
	public class DespawnCurrency : FsmStateAction
	{
		// Token: 0x06007E3B RID: 32315 RVA: 0x00258811 File Offset: 0x00256A11
		public override void Reset()
		{
		}

		// Token: 0x06007E3C RID: 32316 RVA: 0x00258814 File Offset: 0x00256A14
		public override void OnEnter()
		{
			try
			{
				List<CurrencyObjectBase> list = new List<CurrencyObjectBase>();
				list.AddRange(Object.FindObjectsByType<GeoControl>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
				list.AddRange(Object.FindObjectsByType<ShellShard>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Recycle<CurrencyObjectBase>();
				}
			}
			catch (Exception)
			{
			}
			base.Finish();
		}
	}
}
