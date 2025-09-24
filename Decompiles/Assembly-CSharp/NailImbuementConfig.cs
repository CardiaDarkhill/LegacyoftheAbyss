using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001DF RID: 479
[CreateAssetMenu(menuName = "Hornet/Nail Imbuement Config")]
public class NailImbuementConfig : ScriptableObject
{
	// Token: 0x0600129B RID: 4763 RVA: 0x00056600 File Offset: 0x00054800
	public void EnsurePersonalPool(GameObject gameObject)
	{
		if (this.StartHitEffect)
		{
			this.StartHitEffect.EnsurePersonalPool(gameObject);
		}
		if (this.LagHits != null && this.LagHits.LagHitEffect != null)
		{
			this.LagHits.LagHitEffect.EnsurePersonalPool(gameObject);
		}
	}

	// Token: 0x04001163 RID: 4451
	public Color NailTintColor = Color.white;

	// Token: 0x04001164 RID: 4452
	public float Duration;

	// Token: 0x04001165 RID: 4453
	public SpriteFlash.FlashConfig HeroFlashing;

	// Token: 0x04001166 RID: 4454
	public Color ExtraHeroLightColor;

	// Token: 0x04001167 RID: 4455
	public PlayParticleEffects HeroParticles;

	// Token: 0x04001168 RID: 4456
	public EnemyHitEffectsProfile InertHitEffect;

	// Token: 0x04001169 RID: 4457
	public EnemyHitEffectsProfile StartHitEffect;

	// Token: 0x0400116A RID: 4458
	public GameObject SlashEffect;

	// Token: 0x0400116B RID: 4459
	public AudioEvent ExtraSlashAudio;

	// Token: 0x0400116C RID: 4460
	[Space]
	public float NailDamageMultiplier = 1f;

	// Token: 0x0400116D RID: 4461
	[Space]
	public ToolItem ToolSource;

	// Token: 0x0400116E RID: 4462
	[Space]
	public DamageTag DamageTag;

	// Token: 0x0400116F RID: 4463
	[ModifiableProperty]
	[Conditional("DamageTag", true, false, false)]
	public int DamageTagTicksOverride;

	// Token: 0x04001170 RID: 4464
	[Space]
	public MinMaxInt HitsToTag = new MinMaxInt(1, 1);

	// Token: 0x04001171 RID: 4465
	[Range(0f, 1f)]
	public float LuckyHitChance;

	// Token: 0x04001172 RID: 4466
	public MinMaxInt LuckyHitsToTag = new MinMaxInt(1, 1);

	// Token: 0x04001173 RID: 4467
	[Space]
	[ModifiableProperty]
	[Conditional("DamageTag", false, false, false)]
	public NailImbuementConfig.ImbuedLagHitOptions LagHits;

	// Token: 0x0200151F RID: 5407
	[Serializable]
	public class ImbuedLagHitOptions : LagHitOptions
	{
		// Token: 0x17000D5C RID: 3420
		// (get) Token: 0x060085D2 RID: 34258 RVA: 0x00271368 File Offset: 0x0026F568
		public override bool IsExtraDamage
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D5D RID: 3421
		// (get) Token: 0x060085D3 RID: 34259 RVA: 0x0027136B File Offset: 0x0026F56B
		public override bool CanStack
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060085D4 RID: 34260 RVA: 0x00271370 File Offset: 0x0026F570
		public override void OnStart(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance, out ParticleEffectsLerpEmission spawnedHitMarkedEffect)
		{
			if (this.HitMarkedEffect)
			{
				spawnedHitMarkedEffect = this.HitMarkedEffect.Spawn(effectsPoint, effectOrigin);
				spawnedHitMarkedEffect.transform.SetPositionZ(this.HitMarkedEffect.transform.position.z);
				float duration = this.StartDelay + this.HitDelay * (float)this.HitCount;
				spawnedHitMarkedEffect.Play(duration);
				return;
			}
			spawnedHitMarkedEffect = null;
		}

		// Token: 0x060085D5 RID: 34261 RVA: 0x002713E0 File Offset: 0x0026F5E0
		public override void OnHit(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
		{
			if (this.LagHitEffect)
			{
				this.LagHitEffect.SpawnEffects(effectsPoint, effectOrigin, hitInstance, null, -1f);
			}
			SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
			if (component)
			{
				component.Flash(this.EnemyHitFlash);
			}
			this.HitSound.SpawnAndPlayOneShot(effectsPoint.position + effectOrigin, null);
		}

		// Token: 0x040085FC RID: 34300
		public ParticleEffectsLerpEmission HitMarkedEffect;

		// Token: 0x040085FD RID: 34301
		public EnemyHitEffectsProfile LagHitEffect;

		// Token: 0x040085FE RID: 34302
		[Space]
		public SpriteFlash.FlashConfig EnemyHitFlash;

		// Token: 0x040085FF RID: 34303
		public AudioEvent HitSound;
	}
}
