using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class HeroShamanRuneEffect : MonoBehaviour
{
	// Token: 0x06002028 RID: 8232 RVA: 0x000923D0 File Offset: 0x000905D0
	private void Awake()
	{
		if (this.damager)
		{
			this.initialDamageMult = this.damager.DamageMultiplier;
			this.initialAttackType = this.damager.attackType;
		}
		this.zapTintSprites.RemoveNulls<SpriteRenderer>();
		this.zapTintParticles.RemoveNulls<ParticleSystem>();
		if (this.zapTintSprites.Count > 0)
		{
			this.initialSpriteColours = new Dictionary<SpriteRenderer, Color>(this.zapTintSprites.Count);
			foreach (SpriteRenderer spriteRenderer in this.zapTintSprites)
			{
				this.initialSpriteColours[spriteRenderer] = spriteRenderer.color;
			}
		}
		if (this.zapTintParticles.Count > 0)
		{
			this.initialParticleColours = new Dictionary<ParticleSystem, ParticleSystem.MinMaxGradient>(this.zapTintParticles.Count);
			foreach (ParticleSystem particleSystem in this.zapTintParticles)
			{
				this.initialParticleColours[particleSystem] = particleSystem.main.startColor;
			}
		}
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x00092514 File Offset: 0x00090714
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.Refresh();
		}
	}

	// Token: 0x0600202A RID: 8234 RVA: 0x00092524 File Offset: 0x00090724
	private void Start()
	{
		this.hasStarted = true;
		this.Refresh();
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x00092534 File Offset: 0x00090734
	public void Refresh()
	{
		bool isEquipped = Gameplay.SpellCrest.IsEquipped;
		if (this.damager)
		{
			this.damager.DamageMultiplier = (isEquipped ? (this.initialDamageMult * Gameplay.SpellCrestRuneDamageMult) : this.initialDamageMult);
			this.damager.attackType = (isEquipped ? AttackTypes.Spell : this.initialAttackType);
		}
		if (this.rune)
		{
			this.rune.SetActive(isEquipped);
		}
		bool flag = isEquipped && Gameplay.ZapImbuementTool.IsEquipped;
		Color b = flag ? Gameplay.ZapDamageTintColour : Color.white;
		this.disableIfZap.SetAllActive(!flag);
		if (this.initialSpriteColours != null)
		{
			foreach (KeyValuePair<SpriteRenderer, Color> keyValuePair in this.initialSpriteColours)
			{
				keyValuePair.Key.color = keyValuePair.Value * b;
			}
		}
		if (this.initialParticleColours != null)
		{
			foreach (KeyValuePair<ParticleSystem, ParticleSystem.MinMaxGradient> keyValuePair2 in this.initialParticleColours)
			{
				ParticleSystem.MainModule main = keyValuePair2.Key.main;
				ParticleSystem.MinMaxGradient value = keyValuePair2.Value;
				ParticleSystemGradientMode mode = value.mode;
				if (mode != ParticleSystemGradientMode.Color)
				{
					if (mode == ParticleSystemGradientMode.TwoColors)
					{
						value.colorMin *= b;
						value.colorMax *= b;
					}
				}
				else
				{
					value.color *= b;
				}
				main.startColor = value;
			}
		}
		if (isEquipped && this.runeSpawnEffect)
		{
			Transform transform = this.rune ? this.rune.transform : base.transform;
			GameObject gameObject = this.runeSpawnEffect.Spawn(transform.TransformPoint(this.spawnOffset));
			gameObject.transform.localScale = transform.TransformVector(this.spawnScale);
			FollowTransform component = gameObject.GetComponent<FollowTransform>();
			if (component)
			{
				component.Target = transform;
			}
			FollowRotation component2 = gameObject.GetComponent<FollowRotation>();
			if (component2)
			{
				component2.Target = transform;
			}
			ParticleSystem component3 = this.runeSpawnEffect.GetComponent<ParticleSystem>();
			ParticleSystem component4 = gameObject.GetComponent<ParticleSystem>();
			component4.main.startDelay = this.spawnDelay;
			component4.emission.rateOverTimeMultiplier = this.spawnMult * component3.emission.rateOverTimeMultiplier;
			component4.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			component4.Play();
		}
	}

	// Token: 0x04001F24 RID: 7972
	[SerializeField]
	private DamageEnemies damager;

	// Token: 0x04001F25 RID: 7973
	[SerializeField]
	private GameObject rune;

	// Token: 0x04001F26 RID: 7974
	[Space]
	[SerializeField]
	private GameObject runeSpawnEffect;

	// Token: 0x04001F27 RID: 7975
	[SerializeField]
	private Vector3 spawnOffset;

	// Token: 0x04001F28 RID: 7976
	[SerializeField]
	private Vector3 spawnScale = Vector3.one;

	// Token: 0x04001F29 RID: 7977
	[SerializeField]
	private float spawnDelay;

	// Token: 0x04001F2A RID: 7978
	[SerializeField]
	private float spawnMult = 1f;

	// Token: 0x04001F2B RID: 7979
	[Space]
	[SerializeField]
	private List<SpriteRenderer> zapTintSprites;

	// Token: 0x04001F2C RID: 7980
	[SerializeField]
	private List<ParticleSystem> zapTintParticles;

	// Token: 0x04001F2D RID: 7981
	[SerializeField]
	private GameObject[] disableIfZap;

	// Token: 0x04001F2E RID: 7982
	private Dictionary<SpriteRenderer, Color> initialSpriteColours;

	// Token: 0x04001F2F RID: 7983
	private Dictionary<ParticleSystem, ParticleSystem.MinMaxGradient> initialParticleColours;

	// Token: 0x04001F30 RID: 7984
	private bool hasStarted;

	// Token: 0x04001F31 RID: 7985
	private float initialDamageMult;

	// Token: 0x04001F32 RID: 7986
	private AttackTypes initialAttackType;
}
