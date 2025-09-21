using System;
using System.Collections.Generic;
using GlobalSettings;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020001D5 RID: 469
[CreateAssetMenu(menuName = "Hornet/Damage Tag")]
public class DamageTag : ScriptableObject
{
	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06001265 RID: 4709 RVA: 0x000559E0 File Offset: 0x00053BE0
	public float StartDelay
	{
		get
		{
			return this.startDelay;
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06001266 RID: 4710 RVA: 0x000559E8 File Offset: 0x00053BE8
	public float DelayPerHit
	{
		get
		{
			return this.delayPerHit;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06001267 RID: 4711 RVA: 0x000559F0 File Offset: 0x00053BF0
	public int TotalHitLimit
	{
		get
		{
			return this.totalHitLimit;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06001268 RID: 4712 RVA: 0x000559F8 File Offset: 0x00053BF8
	public TimerGroup DamageCooldownTimer
	{
		get
		{
			return this.damageCooldownTimer;
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06001269 RID: 4713 RVA: 0x00055A00 File Offset: 0x00053C00
	public ParticleEffectsLerpEmission CorpseBurnEffect
	{
		get
		{
			return this.corpseBurnEffect;
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x0600126A RID: 4714 RVA: 0x00055A08 File Offset: 0x00053C08
	public DamageTag.SpecialDamageTypes SpecialDamageType
	{
		get
		{
			return this.specialDamageType;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x0600126B RID: 4715 RVA: 0x00055A10 File Offset: 0x00053C10
	public NailElements NailElement
	{
		get
		{
			return this.nailElement;
		}
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00055A18 File Offset: 0x00053C18
	public void OnBegin(ITagDamageTakerOwner owner, out GameObject spawnedLoopEffect)
	{
		Vector2 tagDamageEffectPos = owner.TagDamageEffectPos;
		Vector2 original = owner.transform.TransformPoint(tagDamageEffectPos);
		if (this.startEffect)
		{
			this.startEffect.Spawn(original.ToVector3(this.startEffect.transform.localPosition.z));
		}
		if (this.tagLoopEffect)
		{
			spawnedLoopEffect = this.tagLoopEffect.Spawn(owner.transform, tagDamageEffectPos);
			spawnedLoopEffect.transform.SetPositionZ(this.tagLoopEffect.transform.localPosition.z);
			FollowTransform component = spawnedLoopEffect.GetComponent<FollowTransform>();
			if (component && component.Target != null)
			{
				component.Target = null;
			}
			ParticleEffectsLerpEmission component2 = spawnedLoopEffect.GetComponent<ParticleEffectsLerpEmission>();
			if (component2)
			{
				float duration = this.StartDelay + this.DelayPerHit * (float)this.TotalHitLimit;
				component2.Play(duration);
				return;
			}
		}
		else
		{
			spawnedLoopEffect = null;
		}
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x00055B1C File Offset: 0x00053D1C
	public void OnHit(ITagDamageTakerOwner owner)
	{
		int num = this.damageAmount;
		if (this.isToolDamage)
		{
			float num2 = (float)num;
			float num3 = 1f + (float)PlayerData.instance.ToolKitUpgrades * Gameplay.ToolKitDamageIncrease;
			num = Mathf.RoundToInt(num2 * num3);
		}
		DamageTag.DamageTagInstance damageTagInstance = new DamageTag.DamageTagInstance
		{
			amount = num,
			specialDamageType = this.specialDamageType,
			isHeroDamage = this.isToolDamage,
			nailElements = this.nailElement
		};
		if (!owner.ApplyTagDamage(damageTagInstance))
		{
			return;
		}
		if (this.doFlash)
		{
			SpriteFlash spriteFlash = owner.SpriteFlash;
			if (spriteFlash)
			{
				spriteFlash.Flash(this.flashConfig);
			}
		}
		Vector2 tagDamageEffectPos = owner.TagDamageEffectPos;
		Vector3 position = owner.transform.TransformPoint(tagDamageEffectPos);
		if (this.spawnHitEffect)
		{
			position.z = this.spawnHitEffect.transform.localPosition.z;
			this.spawnHitEffect.Spawn(position);
		}
		this.hitSound.SpawnAndPlayOneShot(position, null);
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x00055C30 File Offset: 0x00053E30
	public void SpawnDeathEffects(Vector3 spawnPosition)
	{
		this.deathBurstEffects.RemoveAll((GameObject o) => o == null);
		foreach (GameObject prefab in this.deathBurstEffects)
		{
			prefab.Spawn(spawnPosition);
		}
	}

	// Token: 0x04001123 RID: 4387
	[SerializeField]
	private int damageAmount;

	// Token: 0x04001124 RID: 4388
	[SerializeField]
	private DamageTag.SpecialDamageTypes specialDamageType;

	// Token: 0x04001125 RID: 4389
	[SerializeField]
	private NailElements nailElement;

	// Token: 0x04001126 RID: 4390
	[SerializeField]
	private bool isToolDamage;

	// Token: 0x04001127 RID: 4391
	[SerializeField]
	private float startDelay;

	// Token: 0x04001128 RID: 4392
	[SerializeField]
	private float delayPerHit;

	// Token: 0x04001129 RID: 4393
	[SerializeField]
	private int totalHitLimit;

	// Token: 0x0400112A RID: 4394
	[SerializeField]
	private TimerGroup damageCooldownTimer;

	// Token: 0x0400112B RID: 4395
	[Space]
	[SerializeField]
	private GameObject startEffect;

	// Token: 0x0400112C RID: 4396
	[SerializeField]
	private GameObject tagLoopEffect;

	// Token: 0x0400112D RID: 4397
	[SerializeField]
	private GameObject spawnHitEffect;

	// Token: 0x0400112E RID: 4398
	[SerializeField]
	private AudioEvent hitSound;

	// Token: 0x0400112F RID: 4399
	[SerializeField]
	private List<GameObject> deathBurstEffects = new List<GameObject>();

	// Token: 0x04001130 RID: 4400
	[SerializeField]
	private bool doFlash;

	// Token: 0x04001131 RID: 4401
	[SerializeField]
	[ModifiableProperty]
	[Conditional("doFlash", true, false, false)]
	private SpriteFlash.FlashConfig flashConfig;

	// Token: 0x04001132 RID: 4402
	[Space]
	[SerializeField]
	private ParticleEffectsLerpEmission corpseBurnEffect;

	// Token: 0x02001518 RID: 5400
	public enum SpecialDamageTypes
	{
		// Token: 0x040085EC RID: 34284
		[UsedImplicitly]
		None,
		// Token: 0x040085ED RID: 34285
		Frost,
		// Token: 0x040085EE RID: 34286
		Lightning
	}

	// Token: 0x02001519 RID: 5401
	public struct DamageTagInstance
	{
		// Token: 0x040085EF RID: 34287
		public int amount;

		// Token: 0x040085F0 RID: 34288
		public DamageTag.SpecialDamageTypes specialDamageType;

		// Token: 0x040085F1 RID: 34289
		public bool isHeroDamage;

		// Token: 0x040085F2 RID: 34290
		public NailElements nailElements;
	}
}
