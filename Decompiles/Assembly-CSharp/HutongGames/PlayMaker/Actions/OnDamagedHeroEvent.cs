using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A5 RID: 4773
	public class OnDamagedHeroEvent : FsmStateAction
	{
		// Token: 0x06007D2E RID: 32046 RVA: 0x002559DE File Offset: 0x00253BDE
		public override void Reset()
		{
			this.Target = null;
			this.SendEvent = null;
		}

		// Token: 0x06007D2F RID: 32047 RVA: 0x002559F0 File Offset: 0x00253BF0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.subscribed = safe.GetComponent<DamageHero>();
			this.subscribed.HeroDamaged += this.OnDamagedHero;
		}

		// Token: 0x06007D30 RID: 32048 RVA: 0x00255A2D File Offset: 0x00253C2D
		public override void OnExit()
		{
			if (this.subscribed == null)
			{
				return;
			}
			this.subscribed.HeroDamaged -= this.OnDamagedHero;
			this.subscribed = null;
		}

		// Token: 0x06007D31 RID: 32049 RVA: 0x00255A5C File Offset: 0x00253C5C
		private void OnDamagedHero()
		{
			this.subscribed.HeroDamaged -= this.OnDamagedHero;
			this.subscribed = null;
			base.Fsm.Event(this.SendEvent);
			base.Finish();
		}

		// Token: 0x04007D31 RID: 32049
		[CheckForComponent(typeof(DamageHero))]
		public FsmOwnerDefault Target;

		// Token: 0x04007D32 RID: 32050
		public FsmEvent SendEvent;

		// Token: 0x04007D33 RID: 32051
		private DamageHero subscribed;
	}
}
