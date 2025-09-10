using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200124E RID: 4686
	public class GetHeroAttackObject : FsmStateAction
	{
		// Token: 0x06007BDC RID: 31708 RVA: 0x00250A50 File Offset: 0x0024EC50
		public override void Reset()
		{
			this.Target = null;
			this.Attack = null;
			this.StoreAttack = null;
		}

		// Token: 0x06007BDD RID: 31709 RVA: 0x00250A68 File Offset: 0x0024EC68
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if (component)
				{
					GameObject gameObject = this.GetGameObject(component);
					this.StoreAttack.Value = gameObject;
				}
			}
			base.Finish();
		}

		// Token: 0x06007BDE RID: 31710 RVA: 0x00250AB4 File Offset: 0x0024ECB4
		private GameObject GetGameObject(HeroController hc)
		{
			HeroController.ConfigGroup currentConfigGroup = hc.CurrentConfigGroup;
			if (currentConfigGroup == null)
			{
				return null;
			}
			switch ((GetHeroAttackObject.AttackObjects)this.Attack.Value)
			{
			case GetHeroAttackObject.AttackObjects.NormalSlash:
				return currentConfigGroup.NormalSlash.gameObject;
			case GetHeroAttackObject.AttackObjects.AlternateSlash:
				return currentConfigGroup.AlternateSlash.gameObject;
			case GetHeroAttackObject.AttackObjects.UpSlash:
				return currentConfigGroup.UpSlash.gameObject;
			case GetHeroAttackObject.AttackObjects.DownSlash:
				return currentConfigGroup.DownSlash.gameObject;
			case GetHeroAttackObject.AttackObjects.WallSlash:
				return currentConfigGroup.WallSlash.gameObject;
			case GetHeroAttackObject.AttackObjects.DashStab:
				return currentConfigGroup.DashStab;
			case GetHeroAttackObject.AttackObjects.DashStabAlt:
				return currentConfigGroup.DashStabAlt;
			case GetHeroAttackObject.AttackObjects.ChargeSlash:
				return currentConfigGroup.ChargeSlash;
			case GetHeroAttackObject.AttackObjects.TauntSlash:
				return currentConfigGroup.TauntSlash;
			}
			throw new NotImplementedException();
		}

		// Token: 0x04007C09 RID: 31753
		public FsmOwnerDefault Target;

		// Token: 0x04007C0A RID: 31754
		[ObjectType(typeof(GetHeroAttackObject.AttackObjects))]
		public FsmEnum Attack;

		// Token: 0x04007C0B RID: 31755
		public FsmGameObject StoreAttack;

		// Token: 0x02001BDB RID: 7131
		public enum AttackObjects
		{
			// Token: 0x04009F2C RID: 40748
			NormalSlash,
			// Token: 0x04009F2D RID: 40749
			AlternateSlash,
			// Token: 0x04009F2E RID: 40750
			UpSlash,
			// Token: 0x04009F2F RID: 40751
			DownSlash,
			// Token: 0x04009F30 RID: 40752
			WallSlash,
			// Token: 0x04009F31 RID: 40753
			DashStab,
			// Token: 0x04009F32 RID: 40754
			DashStabAlt,
			// Token: 0x04009F33 RID: 40755
			ChargeSlash,
			// Token: 0x04009F34 RID: 40756
			TauntSlash = 9
		}
	}
}
