using System;
using System.Collections;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class AbyssWater : MonoBehaviour
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000693 RID: 1683 RVA: 0x000214F8 File Offset: 0x0001F6F8
	private bool IsActive
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

	// Token: 0x06000694 RID: 1684 RVA: 0x00021540 File Offset: 0x0001F740
	private void Start()
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.effectPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.effectPrefab.gameObject, 2, true, false, false);
		}
		this.surfaceWater.HeroEntered += this.OnHeroEnteredWater;
		this.surfaceWater.HeroExited += this.OnHeroExitedWater;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000215AB File Offset: 0x0001F7AB
	private void OnHeroEnteredWater(HeroController hc)
	{
		this.insideHero = hc;
		if (this.effectPrefab)
		{
			base.StartCoroutine(this.EffectFollowHero());
		}
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x000215CE File Offset: 0x0001F7CE
	private void OnHeroExitedWater(HeroController hc)
	{
		this.insideHero = null;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000215D7 File Offset: 0x0001F7D7
	private IEnumerator EffectFollowHero()
	{
		GameObject spawnedEffect = this.effectPrefab.Spawn();
		NestedFadeGroupBase group = spawnedEffect.GetComponent<NestedFadeGroupBase>();
		group.AlphaSelf = 1f;
		ParticleSystem[] particles = spawnedEffect.GetComponentsInChildren<ParticleSystem>(true);
		Transform heroTrans = this.insideHero.transform;
		Transform trans = spawnedEffect.transform;
		trans.position = heroTrans.position;
		do
		{
			trans.position = heroTrans.position;
			yield return null;
		}
		while (this.insideHero);
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (group)
		{
			for (float elapsed = 0f; elapsed < this.effectFadeOutDuration; elapsed += Time.deltaTime)
			{
				trans.position = heroTrans.position;
				float num = elapsed / this.effectFadeOutDuration;
				group.AlphaSelf = 1f - num;
				yield return null;
			}
		}
		for (;;)
		{
			trans.position = heroTrans.position;
			bool flag = false;
			array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
			yield return null;
		}
		spawnedEffect.Recycle();
		spawnedEffect = null;
		yield break;
	}

	// Token: 0x04000663 RID: 1635
	[SerializeField]
	private GameObject effectPrefab;

	// Token: 0x04000664 RID: 1636
	[SerializeField]
	private float effectFadeOutDuration;

	// Token: 0x04000665 RID: 1637
	[Space]
	[SerializeField]
	private SurfaceWaterRegion surfaceWater;

	// Token: 0x04000666 RID: 1638
	[SerializeField]
	[EnumPickerBitmask(typeof(MapZone))]
	private long mapZoneMask;

	// Token: 0x04000667 RID: 1639
	[SerializeField]
	private OverrideBool overrideActive;

	// Token: 0x04000668 RID: 1640
	private HeroController insideHero;
}
