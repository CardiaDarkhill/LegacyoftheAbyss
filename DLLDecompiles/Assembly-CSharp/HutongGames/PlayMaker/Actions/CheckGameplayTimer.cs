using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001398 RID: 5016
	public class CheckGameplayTimer : FSMUtility.CheckFsmStateEveryFrameAction
	{
		// Token: 0x060080BE RID: 32958 RVA: 0x0025F0AA File Offset: 0x0025D2AA
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
		}

		// Token: 0x060080BF RID: 32959 RVA: 0x0025F0BC File Offset: 0x0025D2BC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.timer = (safe ? safe.GetComponent<GameplayTimer>() : null);
			base.OnEnter();
		}

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x060080C0 RID: 32960 RVA: 0x0025F0F3 File Offset: 0x0025D2F3
		public override bool IsTrue
		{
			get
			{
				return this.timer && this.timer.IsTimerComplete;
			}
		}

		// Token: 0x04007FFD RID: 32765
		public FsmOwnerDefault Target;

		// Token: 0x04007FFE RID: 32766
		private GameplayTimer timer;
	}
}
