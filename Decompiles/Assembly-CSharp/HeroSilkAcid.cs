using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003BC RID: 956
public class HeroSilkAcid : MonoBehaviour
{
	// Token: 0x0600202D RID: 8237 RVA: 0x00092812 File Offset: 0x00090A12
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<HeroSilkAcid.SizzleConfig>(ref this.sizzleConfigs, typeof(HeroSilkAcid.SizzleTypes));
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x00092829 File Offset: 0x00090A29
	private void Awake()
	{
		this.OnValidate();
		this.hc = base.GetComponent<HeroController>();
		this.heroFlash = base.GetComponent<SpriteFlash>();
		this.sprite = base.GetComponent<tk2dSprite>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x00092864 File Offset: 0x00090A64
	private void Update()
	{
		int silk = PlayerData.instance.silk;
		foreach (KeyValuePair<HeroSilkAcid.SizzleTypes, HeroSilkAcid.SizzleTracker> keyValuePair in this.sizzles)
		{
			HeroSilkAcid.SizzleTypes sizzleTypes;
			HeroSilkAcid.SizzleTracker sizzleTracker;
			keyValuePair.Deconstruct(out sizzleTypes, out sizzleTracker);
			HeroSilkAcid.SizzleTypes sizzleTypes2 = sizzleTypes;
			HeroSilkAcid.SizzleTracker sizzleTracker2 = sizzleTracker;
			if (!this.isInsideAnyRegion)
			{
				if (sizzleTracker2.SizzleStopCooldownLeft <= 0f)
				{
					break;
				}
				sizzleTracker2.SizzleStopCooldownLeft -= Time.deltaTime;
				if (sizzleTracker2.SizzleStopCooldownLeft <= 0f)
				{
					sizzleTracker2.SizzleTimeLeft = 0f;
				}
				break;
			}
			else
			{
				if (sizzleTracker2.SizzleTimeLeft > 0f)
				{
					Vector3 position = this.hc.transform.position;
					HeroSilkAcid.SizzleConfig sizzleConfig = this.sizzleConfigs[(int)sizzleTypes2];
					if (sizzleConfig.LoopParticles)
					{
						sizzleConfig.LoopParticles.transform.SetPosition2D(position);
					}
					if (sizzleConfig.SilkSizzleParticles)
					{
						sizzleConfig.SilkSizzleParticles.transform.SetPosition2D(position);
					}
					if (sizzleConfig.RequireSilk && silk <= 0)
					{
						sizzleTracker2.SizzleTimeLeft = sizzleTracker2.SizzleTime;
						if (this.hadSilk && sizzleConfig.SilkSizzleParticles)
						{
							sizzleConfig.SilkSizzleParticles.StopParticleSystems();
						}
					}
					else
					{
						if (!this.hadSilk && sizzleConfig.SilkSizzleParticles)
						{
							sizzleConfig.SilkSizzleParticles.PlayParticleSystems();
						}
						sizzleTracker2.SizzleTimeLeft -= Time.deltaTime;
						if (sizzleTracker2.SizzleTimeLeft <= 0f)
						{
							this.TakeSilk(sizzleTracker2, sizzleConfig);
						}
					}
				}
				this.hadSilk = (silk > 0);
			}
		}
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x00092A34 File Offset: 0x00090C34
	private void TakeSilk(HeroSilkAcid.SizzleTracker sizzle, HeroSilkAcid.SizzleConfig config)
	{
		PlayerData instance = PlayerData.instance;
		bool flag = false;
		sizzle.InsideRegionsTemp.AddRange(sizzle.InsideRegions);
		foreach (SilkAcidRegion silkAcidRegion in sizzle.InsideRegionsTemp)
		{
			if (silkAcidRegion.IsProtected)
			{
				silkAcidRegion.Dispel();
			}
			else
			{
				flag = true;
			}
		}
		sizzle.InsideRegionsTemp.Clear();
		if (flag)
		{
			if (instance.silk > 0)
			{
				this.hc.TakeSilk(1);
				if (config.SilkTakeSpawn)
				{
					config.SilkTakeSpawn.Spawn(base.transform.position);
				}
			}
		}
		else if (config.SilkProtectedSpawn)
		{
			config.SilkProtectedSpawn.Spawn(base.transform.position);
		}
		sizzle.InsideRegionsTemp.AddRange(sizzle.InsideRegions);
		foreach (SilkAcidRegion silkAcidRegion2 in sizzle.InsideRegionsTemp)
		{
			silkAcidRegion2.OnTakenSilk(!config.RequireSilk || instance.silk > 0);
		}
		sizzle.InsideRegionsTemp.Clear();
		if (!this.isInsideAnyRegion)
		{
			return;
		}
		if (config.SilkSizzleParticles)
		{
			if (!config.RequireSilk || instance.silk > 0)
			{
				config.SilkSizzleParticles.PlayParticleSystems();
			}
			else
			{
				config.SilkSizzleParticles.StopParticleSystems();
			}
		}
		if (sizzle.QueuedUpdateSizzleTime)
		{
			sizzle.QueuedUpdateSizzleTime = false;
			SilkAcidRegion shortestSizzleRegion = this.GetShortestSizzleRegion(sizzle);
			if (shortestSizzleRegion)
			{
				sizzle.SizzleTime = shortestSizzleRegion.SizzleTime;
			}
		}
		sizzle.SizzleTimeLeft = sizzle.SizzleTime;
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x00092C00 File Offset: 0x00090E00
	private static SilkSpool.SilkUsingFlags MapToUseType(HeroSilkAcid.SizzleTypes sizzleTypes)
	{
		SilkSpool.SilkUsingFlags result;
		if (sizzleTypes != HeroSilkAcid.SizzleTypes.Acid)
		{
			if (sizzleTypes != HeroSilkAcid.SizzleTypes.Void)
			{
				throw new ArgumentOutOfRangeException("sizzleTypes", sizzleTypes, null);
			}
			result = SilkSpool.SilkUsingFlags.Void;
		}
		else
		{
			result = SilkSpool.SilkUsingFlags.Acid;
		}
		return result;
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x00092C34 File Offset: 0x00090E34
	private HeroSilkAcid.SizzleTracker GetSizzleTracker(HeroSilkAcid.SizzleTypes usingType)
	{
		HeroSilkAcid.SizzleTracker result;
		if (!this.sizzles.TryGetValue(usingType, out result))
		{
			result = (this.sizzles[usingType] = new HeroSilkAcid.SizzleTracker());
		}
		return result;
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x00092C68 File Offset: 0x00090E68
	private void StartSizzle(HeroSilkAcid.SizzleTypes sizzleTypes)
	{
		HeroSilkAcid.SizzleTracker sizzleTracker = this.GetSizzleTracker(sizzleTypes);
		if (sizzleTracker.SizzleTimeLeft <= 0f)
		{
			sizzleTracker.SizzleTimeLeft = sizzleTracker.SizzleTime + sizzleTracker.SizzleStartDelay;
		}
		SilkSpool.SilkUsingFlags usingFlags = HeroSilkAcid.MapToUseType(sizzleTypes);
		SilkSpool.Instance.AddUsing(usingFlags, 1);
		HeroSilkAcid.SizzleConfig sizzleConfig = this.sizzleConfigs[(int)sizzleTypes];
		if (sizzleConfig.LoopParticles)
		{
			sizzleConfig.LoopParticles.PlayParticleSystems();
		}
		if ((!sizzleConfig.RequireSilk || PlayerData.instance.silk > 0) && sizzleConfig.SilkSizzleParticles)
		{
			sizzleConfig.SilkSizzleParticles.PlayParticleSystems();
		}
		if (sizzleTracker.FadeRoutine != null)
		{
			base.StopCoroutine(sizzleTracker.FadeRoutine);
			sizzleTracker.FadeRoutine = null;
		}
		if (this.heroFlash)
		{
			if (sizzleTypes != HeroSilkAcid.SizzleTypes.Acid)
			{
				if (sizzleTypes != HeroSilkAcid.SizzleTypes.Void)
				{
					throw new ArgumentOutOfRangeException("sizzleTypes", sizzleTypes, null);
				}
				this.sprite.EnableKeyword("BLACKTHREAD");
				if (this.meshRenderer.enabled)
				{
					this.sprite.SetFloat(HeroSilkAcid._blackThreadAmountProp, 0f);
					sizzleTracker.FadeRoutine = this.StartTimerRoutine(0f, 0.2f, delegate(float t)
					{
						this.sprite.SetFloat(HeroSilkAcid._blackThreadAmountProp, t);
					}, null, null, false);
				}
				else
				{
					this.sprite.SetFloat(HeroSilkAcid._blackThreadAmountProp, 1f);
				}
			}
			else
			{
				sizzleTracker.HeroFlashHandle = this.heroFlash.Flash(new Color(0.9647059f, 0.9372549f, 0.42352942f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 2, false);
			}
		}
		this.hadSilk = (PlayerData.instance.silk > 0);
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x00092E14 File Offset: 0x00091014
	private void StopSizzle(HeroSilkAcid.SizzleTypes sizzleTypes)
	{
		SilkSpool.SilkUsingFlags usingFlags = HeroSilkAcid.MapToUseType(sizzleTypes);
		SilkSpool.Instance.RemoveUsing(usingFlags, 1);
		HeroSilkAcid.SizzleConfig sizzleConfig = this.sizzleConfigs[(int)sizzleTypes];
		if (sizzleConfig.LoopParticles)
		{
			sizzleConfig.LoopParticles.StopParticleSystems();
		}
		if (sizzleConfig.SilkSizzleParticles)
		{
			sizzleConfig.SilkSizzleParticles.StopParticleSystems();
		}
		HeroSilkAcid.SizzleTracker sizzleTracker;
		if (!this.sizzles.TryGetValue(sizzleTypes, out sizzleTracker))
		{
			Debug.LogError("No sizzle was ever started for type: " + sizzleTypes.ToString());
			return;
		}
		if (this.heroFlash)
		{
			this.heroFlash.CancelRepeatingFlash(sizzleTracker.HeroFlashHandle);
		}
		if (sizzleTracker.FadeRoutine != null)
		{
			base.StopCoroutine(sizzleTracker.FadeRoutine);
			sizzleTracker.FadeRoutine = null;
		}
		if (sizzleTypes == HeroSilkAcid.SizzleTypes.Void)
		{
			sizzleTracker.FadeRoutine = this.StartTimerRoutine(0f, 0.5f, delegate(float t)
			{
				this.sprite.SetFloat(HeroSilkAcid._blackThreadAmountProp, 1f - t);
			}, null, delegate
			{
				this.sprite.DisableKeyword("BLACKTHREAD");
			}, false);
		}
		if (sizzleTracker.SizzleTimeLeft > 0f && this.sizzleStopCooldown > 0f)
		{
			sizzleTracker.SizzleStopCooldownLeft = this.sizzleStopCooldown;
			return;
		}
		sizzleTracker.SizzleTimeLeft = 0f;
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x00092F3C File Offset: 0x0009113C
	public void AddInside(SilkAcidRegion region)
	{
		HeroSilkAcid.SizzleTracker sizzleTracker = this.GetSizzleTracker(region.SizzleType);
		bool flag = sizzleTracker.InsideRegions.Count == 0;
		sizzleTracker.InsideRegions.AddIfNotPresent(region);
		sizzleTracker.QueuedUpdateSizzleTime = true;
		if (flag)
		{
			this.isInsideAnyRegion = true;
			this.hc.SetSilkRegenBlocked(true);
			sizzleTracker.SizzleStartDelay = region.SizzleStartDelay;
			sizzleTracker.SizzleTime = region.SizzleTime;
			this.StartSizzle(region.SizzleType);
			if (region.SizzleType == HeroSilkAcid.SizzleTypes.Void)
			{
				StatusVignette.AddStatus(StatusVignette.StatusTypes.Voided);
			}
		}
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x00092FC4 File Offset: 0x000911C4
	public void RemoveInside(SilkAcidRegion region)
	{
		HeroSilkAcid.SizzleTracker sizzleTracker = this.GetSizzleTracker(region.SizzleType);
		sizzleTracker.InsideRegions.Remove(region);
		sizzleTracker.QueuedUpdateSizzleTime = true;
		if (sizzleTracker.InsideRegions.Count == 0)
		{
			this.isInsideAnyRegion = false;
			this.hc.SetSilkRegenBlocked(false);
			this.StopSizzle(region.SizzleType);
			if (region.SizzleType == HeroSilkAcid.SizzleTypes.Void)
			{
				StatusVignette.RemoveStatus(StatusVignette.StatusTypes.Voided);
			}
		}
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x0009302C File Offset: 0x0009122C
	private SilkAcidRegion GetShortestSizzleRegion(HeroSilkAcid.SizzleTracker sizzle)
	{
		float num = float.MaxValue;
		SilkAcidRegion result = null;
		foreach (SilkAcidRegion silkAcidRegion in sizzle.InsideRegions)
		{
			if (silkAcidRegion.SizzleTime < num)
			{
				num = silkAcidRegion.SizzleTime;
				result = silkAcidRegion;
			}
		}
		return result;
	}

	// Token: 0x04001F33 RID: 7987
	[SerializeField]
	[ArrayForEnum(typeof(HeroSilkAcid.SizzleTypes))]
	private HeroSilkAcid.SizzleConfig[] sizzleConfigs;

	// Token: 0x04001F34 RID: 7988
	[Space]
	[SerializeField]
	private float sizzleStopCooldown;

	// Token: 0x04001F35 RID: 7989
	private HeroController hc;

	// Token: 0x04001F36 RID: 7990
	private SpriteFlash heroFlash;

	// Token: 0x04001F37 RID: 7991
	private tk2dSprite sprite;

	// Token: 0x04001F38 RID: 7992
	private MeshRenderer meshRenderer;

	// Token: 0x04001F39 RID: 7993
	private bool isInsideAnyRegion;

	// Token: 0x04001F3A RID: 7994
	private bool hadSilk;

	// Token: 0x04001F3B RID: 7995
	private readonly Dictionary<HeroSilkAcid.SizzleTypes, HeroSilkAcid.SizzleTracker> sizzles = new Dictionary<HeroSilkAcid.SizzleTypes, HeroSilkAcid.SizzleTracker>();

	// Token: 0x04001F3C RID: 7996
	private static readonly int _blackThreadAmountProp = Shader.PropertyToID("_BlackThreadAmount");

	// Token: 0x0200166F RID: 5743
	private class SizzleTracker
	{
		// Token: 0x04008ACD RID: 35533
		public float SizzleStartDelay;

		// Token: 0x04008ACE RID: 35534
		public float SizzleTime;

		// Token: 0x04008ACF RID: 35535
		public float SizzleTimeLeft;

		// Token: 0x04008AD0 RID: 35536
		public float SizzleStopCooldownLeft;

		// Token: 0x04008AD1 RID: 35537
		public bool QueuedUpdateSizzleTime;

		// Token: 0x04008AD2 RID: 35538
		public readonly List<SilkAcidRegion> InsideRegions = new List<SilkAcidRegion>();

		// Token: 0x04008AD3 RID: 35539
		public readonly List<SilkAcidRegion> InsideRegionsTemp = new List<SilkAcidRegion>();

		// Token: 0x04008AD4 RID: 35540
		public SpriteFlash.FlashHandle HeroFlashHandle;

		// Token: 0x04008AD5 RID: 35541
		public Coroutine FadeRoutine;
	}

	// Token: 0x02001670 RID: 5744
	[Serializable]
	private class SizzleConfig
	{
		// Token: 0x04008AD6 RID: 35542
		public bool RequireSilk;

		// Token: 0x04008AD7 RID: 35543
		public PlayParticleEffects LoopParticles;

		// Token: 0x04008AD8 RID: 35544
		public PlayParticleEffects SilkSizzleParticles;

		// Token: 0x04008AD9 RID: 35545
		public GameObject SilkTakeSpawn;

		// Token: 0x04008ADA RID: 35546
		public GameObject SilkProtectedSpawn;
	}

	// Token: 0x02001671 RID: 5745
	public enum SizzleTypes
	{
		// Token: 0x04008ADC RID: 35548
		Acid,
		// Token: 0x04008ADD RID: 35549
		Void
	}
}
