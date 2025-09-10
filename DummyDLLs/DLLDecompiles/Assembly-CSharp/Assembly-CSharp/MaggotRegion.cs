using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class MaggotRegion : MonoBehaviour
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x060006DE RID: 1758 RVA: 0x000228FB File Offset: 0x00020AFB
	[UsedImplicitly]
	public static bool IsInsideAny
	{
		get
		{
			return MaggotRegion._insideRegions.Count > 0;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x060006DF RID: 1759 RVA: 0x0002290C File Offset: 0x00020B0C
	public bool IsActive
	{
		get
		{
			if (this.overrideActive.IsEnabled)
			{
				if (!this.overrideActive.Value)
				{
					return false;
				}
			}
			else
			{
				MapZone currentMapZoneEnum = GameManager.instance.GetCurrentMapZoneEnum();
				if (!this.mapZoneMask.IsBitSet((int)currentMapZoneEnum))
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00022954 File Offset: 0x00020B54
	private void Start()
	{
		if (this.IsActive)
		{
			if (this.heroMaggotsPrefab)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.heroMaggotsPrefab.gameObject, 5, false, false, false);
			}
			if (this.heroMaggotShieldPrefab)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.heroMaggotShieldPrefab.gameObject, 3, false, false, false);
			}
			PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
			this.surfaceWater.HeroEntered += this.OnHeroEnteredWater;
			this.surfaceWater.HeroExited += this.OnHeroExitedWater;
			this.surfaceWater.CorpseEntered += this.OnOtherEnteredWater;
		}
		else if (!this.forceSwampColour)
		{
			return;
		}
		this.surfaceWater.Color = Effects.MossEffectsTintDust;
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00022A25 File Offset: 0x00020C25
	private void OnDisable()
	{
		MaggotRegion._insideRegions.Remove(this);
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00022A34 File Offset: 0x00020C34
	private void OnHeroEnteredWater(HeroController hc)
	{
		this.insideHero = hc;
		MaggotRegion._insideRegions.Add(this);
		EventRegister.SendEvent(EventRegisterEvents.MaggotCheck, null);
		if (Gameplay.MaggotCharm.IsEquipped && this.insideHero.playerData.MaggotCharmHits < 3)
		{
			this.insideHero.AddToMaggotCharmTimer(Gameplay.MaggotCharmEnterWaterAddTime);
			this.heroMaggotsRoutine = base.StartCoroutine(this.HeroInMaggotRegion());
			return;
		}
		this.StartHeroMaggoted();
		this.heroMaggotsRoutine = base.StartCoroutine(this.HeroInMaggotRegion());
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00022AB8 File Offset: 0x00020CB8
	private void StartHeroMaggoted()
	{
		this.isMaggoted = true;
		SilkSpool.Instance.AddUsing(SilkSpool.SilkUsingFlags.Maggot, 1);
		this.insideHero.SetSilkRegenBlocked(true);
		this.takeSilkRoutine = base.StartCoroutine(this.TakeSilk());
		MaggotRegion.SetIsMaggoted(true);
		StatusVignette.AddStatus(StatusVignette.StatusTypes.InMaggotRegion);
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00022AF8 File Offset: 0x00020CF8
	private void OnHeroExitedWater(HeroController hc)
	{
		this.insideHero = null;
		MaggotRegion._insideRegions.Remove(this);
		EventRegister.SendEvent(EventRegisterEvents.MaggotCheck, null);
		if (this.heroMaggotsRoutine != null)
		{
			base.StopCoroutine(this.heroMaggotsRoutine);
			this.heroMaggotsRoutine = null;
			if (this.spawnedHeroMaggots)
			{
				this.spawnedHeroMaggots.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				FadeOutAudioSource component = this.spawnedHeroMaggots.GetComponent<FadeOutAudioSource>();
				if (component)
				{
					component.StartFade(0.5f);
				}
				this.spawnedHeroMaggots = null;
			}
			this.EndHeroShield();
		}
		if (!this.isMaggoted)
		{
			return;
		}
		this.isMaggoted = false;
		StatusVignette.RemoveStatus(StatusVignette.StatusTypes.InMaggotRegion);
		SilkSpool.Instance.RemoveUsing(SilkSpool.SilkUsingFlags.Maggot, 1);
		hc.SetSilkRegenBlocked(false);
		base.StopCoroutine(this.takeSilkRoutine);
		this.takeSilkRoutine = null;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00022BC4 File Offset: 0x00020DC4
	private void EndHeroShield()
	{
		if (!this.spawnedHeroShield)
		{
			return;
		}
		this.spawnedHeroShield.SetTrigger(MaggotRegion._endId);
		AutoRecycleSelf component = this.spawnedHeroShield.GetComponent<AutoRecycleSelf>();
		if (component)
		{
			component.ActivateTimer();
		}
		this.spawnedHeroShield = null;
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00022C10 File Offset: 0x00020E10
	private void OnOtherEnteredWater(Vector2 position)
	{
		if (!this.otherMaggotsPrefab)
		{
			return;
		}
		ParticleSystem particleSystem = this.otherMaggotsPrefab.Spawn<ParticleSystem>();
		particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particleSystem.transform.SetPosition2D(position);
		particleSystem.Play(true);
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00022C45 File Offset: 0x00020E45
	private IEnumerator TakeSilk()
	{
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		float num = (float)(Time.timeAsDouble - this.lastSilkTime);
		float num2 = 0.5f - num;
		if (num2 > -1f)
		{
			if (num2 > 0f)
			{
				yield return new WaitForSeconds(num2);
			}
		}
		else
		{
			this.lastSilkTime = Time.timeAsDouble;
			yield return wait;
		}
		for (;;)
		{
			this.insideHero.TakeSilk(1);
			this.lastSilkTime = Time.timeAsDouble;
			yield return wait;
		}
		yield break;
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00022C54 File Offset: 0x00020E54
	private IEnumerator HeroInMaggotRegion()
	{
		Transform heroTrans = this.insideHero.transform;
		Transform heroMaggotsTrans;
		if (this.heroMaggotsPrefab)
		{
			this.spawnedHeroMaggots = this.heroMaggotsPrefab.Spawn<ParticleSystem>();
			this.spawnedHeroMaggots.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			heroMaggotsTrans = this.spawnedHeroMaggots.transform;
			heroMaggotsTrans.SetPosition2D(heroTrans.position.x, base.transform.position.y + this.heroMaggotsYPos);
			this.spawnedHeroMaggots.Play(true);
		}
		else
		{
			heroMaggotsTrans = null;
		}
		Transform heroShieldTrans;
		if (!this.isMaggoted && this.heroMaggotShieldPrefab)
		{
			this.spawnedHeroShield = this.heroMaggotShieldPrefab.Spawn(heroTrans.position);
			heroShieldTrans = this.spawnedHeroShield.transform;
			heroShieldTrans.SetPositionZ(this.heroMaggotShieldPrefab.transform.position.z);
		}
		else
		{
			heroShieldTrans = null;
		}
		for (;;)
		{
			Vector3 position = heroTrans.position;
			if (heroMaggotsTrans)
			{
				heroMaggotsTrans.SetPositionX(position.x);
			}
			if (heroShieldTrans)
			{
				heroShieldTrans.SetPosition2D(position);
			}
			if (!this.isMaggoted)
			{
				this.insideHero.AddToMaggotCharmTimer(Time.deltaTime);
				if (this.insideHero.playerData.MaggotCharmHits >= 3)
				{
					this.EndHeroShield();
					this.StartHeroMaggoted();
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00022C63 File Offset: 0x00020E63
	public static void SetIsMaggoted(bool value)
	{
		HeroController.instance.SetIsMaggoted(value);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00022C70 File Offset: 0x00020E70
	public void ReportExplosion(Vector2 position)
	{
		if (!this.IsActive)
		{
			return;
		}
		this.maggotJournalRecord.Get(this.maggotJournalRecordAmount.GetRandomValue(true), true);
		if (!this.maggotKillEffect)
		{
			return;
		}
		position.y = base.transform.position.y + this.heroMaggotsYPos;
		ParticleSystem particleSystem = this.maggotKillEffect.Spawn<ParticleSystem>();
		particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particleSystem.transform.SetPosition2D(position);
		particleSystem.Play(true);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00022CF0 File Offset: 0x00020EF0
	public void ReportLightningExplosion(Vector2 position)
	{
		if (!this.IsActive)
		{
			return;
		}
		this.maggotJournalRecord.Get(this.lightningKillAmount.GetRandomValue(true), true);
		if (!this.lightningKillEffect)
		{
			return;
		}
		position.y = base.transform.position.y + this.heroMaggotsYPos;
		ParticleSystem particleSystem = this.lightningKillEffect.Spawn<ParticleSystem>();
		particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particleSystem.transform.SetPosition2D(position);
		particleSystem.Play(true);
	}

	// Token: 0x040006BD RID: 1725
	[SerializeField]
	private ParticleSystem heroMaggotsPrefab;

	// Token: 0x040006BE RID: 1726
	[SerializeField]
	private ParticleSystem otherMaggotsPrefab;

	// Token: 0x040006BF RID: 1727
	[SerializeField]
	private float heroMaggotsYPos;

	// Token: 0x040006C0 RID: 1728
	[SerializeField]
	private Animator heroMaggotShieldPrefab;

	// Token: 0x040006C1 RID: 1729
	[Space]
	[SerializeField]
	private EnemyJournalRecord maggotJournalRecord;

	// Token: 0x040006C2 RID: 1730
	[SerializeField]
	private MinMaxInt maggotJournalRecordAmount;

	// Token: 0x040006C3 RID: 1731
	[SerializeField]
	private ParticleSystem maggotKillEffect;

	// Token: 0x040006C4 RID: 1732
	[Space]
	[SerializeField]
	private MinMaxInt lightningKillAmount = new MinMaxInt(3, 4);

	// Token: 0x040006C5 RID: 1733
	[SerializeField]
	private ParticleSystem lightningKillEffect;

	// Token: 0x040006C6 RID: 1734
	[Space]
	[SerializeField]
	private SurfaceWaterRegion surfaceWater;

	// Token: 0x040006C7 RID: 1735
	[SerializeField]
	[EnumPickerBitmask(typeof(MapZone))]
	private long mapZoneMask;

	// Token: 0x040006C8 RID: 1736
	[SerializeField]
	private OverrideBool overrideActive;

	// Token: 0x040006C9 RID: 1737
	[SerializeField]
	private bool forceSwampColour;

	// Token: 0x040006CA RID: 1738
	private const float TAKE_SILK_DELAY = 0.5f;

	// Token: 0x040006CB RID: 1739
	private const float EXIT_COOLDOWN = 1f;

	// Token: 0x040006CC RID: 1740
	private static readonly int _endId = Animator.StringToHash("End");

	// Token: 0x040006CD RID: 1741
	private HeroController insideHero;

	// Token: 0x040006CE RID: 1742
	private bool isMaggoted;

	// Token: 0x040006CF RID: 1743
	private double lastSilkTime;

	// Token: 0x040006D0 RID: 1744
	private Coroutine takeSilkRoutine;

	// Token: 0x040006D1 RID: 1745
	private Coroutine heroMaggotsRoutine;

	// Token: 0x040006D2 RID: 1746
	private ParticleSystem spawnedHeroMaggots;

	// Token: 0x040006D3 RID: 1747
	private Animator spawnedHeroShield;

	// Token: 0x040006D4 RID: 1748
	private static readonly List<MaggotRegion> _insideRegions = new List<MaggotRegion>();
}
