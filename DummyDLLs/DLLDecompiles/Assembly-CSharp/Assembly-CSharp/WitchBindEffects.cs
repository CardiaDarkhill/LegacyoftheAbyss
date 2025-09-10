using System;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003C3 RID: 963
public class WitchBindEffects : MonoBehaviour
{
	// Token: 0x0600209E RID: 8350 RVA: 0x00095F28 File Offset: 0x00094128
	private void Awake()
	{
		this.gameManager = GameManager.instance;
		this.heroController = HeroController.instance;
		this.heroSpriteFlash = this.heroController.GetComponent<SpriteFlash>();
		this.damagers = base.GetComponentsInChildren<DamageEnemies>(true);
		this.hasDamaged = new bool[this.damagers.Length];
		for (int i = 0; i < this.damagers.Length; i++)
		{
			DamageEnemies damageEnemies = this.damagers[i];
			damageEnemies.DamagedEnemyHealthManager += this.OnDamagedEnemyHealthManager;
			int damagerIndex = i;
			damageEnemies.DamagedEnemy += delegate()
			{
				if (this.hasDamaged[damagerIndex])
				{
					return;
				}
				this.hasDamaged[damagerIndex] = true;
				this.gameManager.FreezeMoment(FreezeMomentTypes.WitchBindHit, null);
				GlobalSettings.Camera.MainCameraShakeManager.DoShake(GlobalSettings.Camera.EnemyKillShake, this, true, true, true);
				this.healFlashEffectPrefab.Spawn(this.transform.position + new Vector3(0f, 0f, -0.21f));
			};
		}
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x00095FD0 File Offset: 0x000941D0
	private void OnEnable()
	{
		for (int i = 0; i < this.hasDamaged.Length; i++)
		{
			this.hasDamaged[i] = false;
		}
		this.healCapLeft = this.heroController.GetWitchHealCap();
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x0009600A File Offset: 0x0009420A
	private void OnDamagedEnemyHealthManager(HealthManager hm)
	{
		if (!hm)
		{
			return;
		}
		if (!hm.ShouldIgnore(HealthManager.IgnoreFlags.WitchHeal))
		{
			this.OnDamagedEnemy();
		}
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x00096024 File Offset: 0x00094224
	private void OnDamagedEnemy()
	{
		if (this.healCapLeft <= 0)
		{
			return;
		}
		this.heroController.AddHealth(1);
		this.healCapLeft--;
		this.heroSpriteFlash.flashFocusHeal();
		if (this.healEffects.Emitter)
		{
			this.healEffects.Emitter.Emit(this.healEffects.Emit.GetRandomValue(true));
		}
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x00096094 File Offset: 0x00094294
	public void CancelDamage()
	{
		if (this.hasDamaged == null)
		{
			return;
		}
		for (int i = 0; i < this.hasDamaged.Length; i++)
		{
			this.hasDamaged[i] = true;
		}
		this.healCapLeft = 0;
	}

	// Token: 0x04001FD7 RID: 8151
	[SerializeField]
	private WitchBindEffects.HealEffects healEffects;

	// Token: 0x04001FD8 RID: 8152
	[SerializeField]
	private GameObject healFlashEffectPrefab;

	// Token: 0x04001FD9 RID: 8153
	private bool[] hasDamaged;

	// Token: 0x04001FDA RID: 8154
	private DamageEnemies[] damagers;

	// Token: 0x04001FDB RID: 8155
	private int healCapLeft;

	// Token: 0x04001FDC RID: 8156
	private GameManager gameManager;

	// Token: 0x04001FDD RID: 8157
	private HeroController heroController;

	// Token: 0x04001FDE RID: 8158
	private SpriteFlash heroSpriteFlash;

	// Token: 0x0200167B RID: 5755
	[Serializable]
	private struct HealEffects
	{
		// Token: 0x04008AFF RID: 35583
		public ParticleSystem Emitter;

		// Token: 0x04008B00 RID: 35584
		public MinMaxInt Emit;
	}
}
