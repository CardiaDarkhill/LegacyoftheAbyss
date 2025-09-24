using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AFB RID: 2811
	[ActionCategory("Hollow Knight")]
	public class DamageEnemyRespond : FsmStateAction
	{
		// Token: 0x0600590D RID: 22797 RVA: 0x001C3CA2 File Offset: 0x001C1EA2
		public override void Reset()
		{
			this.Target = null;
			this.DamagedEvent = null;
			this.StoreEnemy = null;
		}

		// Token: 0x0600590E RID: 22798 RVA: 0x001C3CBC File Offset: 0x001C1EBC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.damager = safe.GetComponent<DamageEnemies>();
				if (this.damager)
				{
					this.damager.DamagedEnemyGameObject += this.OnCausedDamage;
					this.damager.DamagedEnemy += this.OnCausedDamage;
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x0600590F RID: 22799 RVA: 0x001C3D2C File Offset: 0x001C1F2C
		public override void OnExit()
		{
			if (this.damager)
			{
				this.damager.DamagedEnemyGameObject -= this.OnCausedDamage;
				this.damager.DamagedEnemy -= this.OnCausedDamage;
			}
		}

		// Token: 0x06005910 RID: 22800 RVA: 0x001C3D69 File Offset: 0x001C1F69
		private void OnCausedDamage(GameObject enemy)
		{
			this.StoreEnemy.Value = enemy;
		}

		// Token: 0x06005911 RID: 22801 RVA: 0x001C3D77 File Offset: 0x001C1F77
		private void OnCausedDamage()
		{
			base.Fsm.Event(this.DamagedEvent);
		}

		// Token: 0x0400542C RID: 21548
		public FsmOwnerDefault Target;

		// Token: 0x0400542D RID: 21549
		public FsmEvent DamagedEvent;

		// Token: 0x0400542E RID: 21550
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreEnemy;

		// Token: 0x0400542F RID: 21551
		private DamageEnemies damager;
	}
}
