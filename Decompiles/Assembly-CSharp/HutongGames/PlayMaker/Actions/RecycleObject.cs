using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF6 RID: 3318
	[ActionCategory(ActionCategory.GameObject)]
	public class RecycleObject : FsmStateAction
	{
		// Token: 0x0600626D RID: 25197 RVA: 0x001F252E File Offset: 0x001F072E
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x001F2538 File Offset: 0x001F0738
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				value.Recycle();
			}
			base.Finish();
		}

		// Token: 0x040060D8 RID: 24792
		[RequiredField]
		public FsmGameObject gameObject;
	}
}
