using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002FF RID: 767
[Serializable]
public class LagHitOptions
{
	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001B6D RID: 7021 RVA: 0x00080243 File Offset: 0x0007E443
	public virtual bool IsExtraDamage
	{
		get
		{
			return this.DamageType != LagHitDamageType.Slash;
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001B6E RID: 7022 RVA: 0x00080251 File Offset: 0x0007E451
	public virtual bool CanStack
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x00080254 File Offset: 0x0007E454
	public bool ShouldDoLagHits()
	{
		return this.HitCount > 0;
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x00080260 File Offset: 0x0007E460
	public virtual void OnStart(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance, out ParticleEffectsLerpEmission spawnedHitMarkedEffect)
	{
		spawnedHitMarkedEffect = null;
		if (this.DamageType == LagHitDamageType.WitchPoison)
		{
			GameObject enemyWitchPoisonHitEffectPrefab = Effects.EnemyWitchPoisonHitEffectPrefab;
			if (enemyWitchPoisonHitEffectPrefab)
			{
				float hitDirectionAsAngle = hitInstance.GetHitDirectionAsAngle(HitInstance.TargetType.Regular);
				enemyWitchPoisonHitEffectPrefab.Spawn(effectsPoint.TransformPoint(effectOrigin), Quaternion.Euler(0f, 0f, hitDirectionAsAngle));
			}
			SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
			if (component)
			{
				component.FlashWitchPoison();
			}
		}
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000802C4 File Offset: 0x0007E4C4
	public virtual void OnHit(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
	{
		LagHitDamageType damageType = this.DamageType;
		if (damageType == LagHitDamageType.WitchPoison)
		{
			this.DoPoisonEffects(effectsPoint, effectOrigin);
			return;
		}
		if (damageType != LagHitDamageType.Dazzle)
		{
			return;
		}
		SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
		if (component)
		{
			component.FlashDazzleQuick();
		}
		Vector3 vector = effectsPoint.TransformPoint(effectOrigin);
		Effects.EnemyDamageTickSoundTable.SpawnAndPlayOneShot(vector, false);
		foreach (GameObject gameObject in this.SlashEffectOverrides)
		{
			GameObject prefab = gameObject;
			Vector3 original = vector;
			float? z = new float?(gameObject.transform.localPosition.z);
			prefab.Spawn(original.Where(null, null, z));
		}
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x00080370 File Offset: 0x0007E570
	private void DoPoisonEffects(Transform effectsPoint, Vector3 effectOrigin)
	{
		Vector3 vector = effectsPoint.TransformPoint(effectOrigin);
		SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
		if (component)
		{
			component.FlashWitchPoison();
		}
		GameObject enemyWitchPoisonHurtEffectPrefab = Effects.EnemyWitchPoisonHurtEffectPrefab;
		if (enemyWitchPoisonHurtEffectPrefab)
		{
			GameObject prefab = enemyWitchPoisonHurtEffectPrefab;
			Vector3 original = vector;
			float? z = new float?(enemyWitchPoisonHurtEffectPrefab.transform.localPosition.z);
			FollowTransform component2 = prefab.Spawn(original.Where(null, null, z)).GetComponent<FollowTransform>();
			if (component2)
			{
				component2.Target = effectsPoint;
			}
		}
		BloodSpawner.SpawnBlood(Effects.EnemyWitchPoisonBloodBurst, vector);
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x00080402 File Offset: 0x0007E602
	public virtual void OnEnd(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
	{
	}

	// Token: 0x04001A67 RID: 6759
	public bool UseNailDamage;

	// Token: 0x04001A68 RID: 6760
	[ModifiableProperty]
	[Conditional("UseNailDamage", true, false, false)]
	public float NailDamageMultiplier = 1f;

	// Token: 0x04001A69 RID: 6761
	[ModifiableProperty]
	[Conditional("UseNailDamage", false, false, false)]
	public int HitDamage;

	// Token: 0x04001A6A RID: 6762
	public LagHitDamageType DamageType;

	// Token: 0x04001A6B RID: 6763
	public float StartDelay;

	// Token: 0x04001A6C RID: 6764
	public int HitCount;

	// Token: 0x04001A6D RID: 6765
	public float HitDelay;

	// Token: 0x04001A6E RID: 6766
	public float MagnitudeMult = 1f;

	// Token: 0x04001A6F RID: 6767
	public bool HitsGiveSilk;

	// Token: 0x04001A70 RID: 6768
	public GameObject[] SlashEffectOverrides;

	// Token: 0x04001A71 RID: 6769
	public bool IgnoreBlock;
}
