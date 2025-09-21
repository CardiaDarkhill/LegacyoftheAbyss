using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class UnMaggotRegion : MonoBehaviour
{
	// Token: 0x0600079A RID: 1946 RVA: 0x00024D84 File Offset: 0x00022F84
	private void Start()
	{
		if (this.maggotRegion && this.maggotRegion.IsActive)
		{
			return;
		}
		if (this.surfaceWater)
		{
			this.surfaceWater.HeroEntered += this.OnHeroEnteredWater;
			this.surfaceWater.HeroExited += this.OnHeroExitedWater;
		}
		else if (this.alertRange)
		{
			this.alertRange.InsideStateChanged += delegate(bool isInside)
			{
				if (isInside)
				{
					this.Entered();
					return;
				}
				this.Exited();
			};
		}
		if (this.heroMaggotsPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.heroMaggotsPrefab.gameObject, 3, true, false, false);
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x00024E34 File Offset: 0x00023034
	private void OnHeroEnteredWater(HeroController hc)
	{
		this.insideHero = hc;
		this.Entered();
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00024E44 File Offset: 0x00023044
	private void Entered()
	{
		HeroController heroController = this.insideHero ? this.insideHero : HeroController.instance;
		if (heroController.cState.isMaggoted && this.unMaggotRoutine == null)
		{
			this.unMaggotRoutine = base.StartCoroutine(this.UnMaggot(heroController));
		}
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00024E94 File Offset: 0x00023094
	private void OnHeroExitedWater(HeroController hc)
	{
		this.insideHero = null;
		this.Exited();
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00024EA3 File Offset: 0x000230A3
	private void Exited()
	{
		if (this.unMaggotRoutine != null)
		{
			base.StopCoroutine(this.unMaggotRoutine);
			this.unMaggotRoutine = null;
		}
		this.StopMaggots();
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x00024EC8 File Offset: 0x000230C8
	private void StopMaggots()
	{
		if (this.spawnedHeroMaggots == null)
		{
			return;
		}
		this.spawnedHeroMaggots.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		FadeOutAudioSource component = this.spawnedHeroMaggots.GetComponent<FadeOutAudioSource>();
		if (component)
		{
			component.StartFade(0.5f);
		}
		this.spawnedHeroMaggots = null;
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00024F17 File Offset: 0x00023117
	private IEnumerator UnMaggot(HeroController hero)
	{
		this.spawnedHeroMaggots = this.heroMaggotsPrefab.Spawn<ParticleSystem>();
		this.spawnedHeroMaggots.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		Transform heroTrans = hero.transform;
		Transform trans = this.spawnedHeroMaggots.transform;
		Vector3 position = heroTrans.position;
		float num = this.positionRelativeToHero ? position.y : base.transform.position.y;
		trans.SetPosition2D(position.x, num + this.heroMaggotsYPos);
		this.spawnedHeroMaggots.Play(true);
		for (float elapsed = 0f; elapsed < 2f; elapsed += Time.deltaTime)
		{
			if (trans != null)
			{
				trans.SetPositionX(heroTrans.position.x);
			}
			yield return null;
		}
		this.StopMaggots();
		Vector3 position2 = heroTrans.position;
		this.maggotsBurstPrefab.Spawn(position2);
		this.maggotsBurstAudio.SpawnAndPlayOneShot(position2, null);
		MaggotRegion.SetIsMaggoted(false);
		this.unMaggotRoutine = null;
		yield break;
	}

	// Token: 0x04000768 RID: 1896
	[SerializeField]
	private SurfaceWaterRegion surfaceWater;

	// Token: 0x04000769 RID: 1897
	[SerializeField]
	private MaggotRegion maggotRegion;

	// Token: 0x0400076A RID: 1898
	[Space]
	[SerializeField]
	private AlertRange alertRange;

	// Token: 0x0400076B RID: 1899
	[Space]
	[SerializeField]
	private ParticleSystem heroMaggotsPrefab;

	// Token: 0x0400076C RID: 1900
	[SerializeField]
	private float heroMaggotsYPos;

	// Token: 0x0400076D RID: 1901
	[SerializeField]
	private bool positionRelativeToHero;

	// Token: 0x0400076E RID: 1902
	[Space]
	[SerializeField]
	private GameObject maggotsBurstPrefab;

	// Token: 0x0400076F RID: 1903
	[SerializeField]
	private AudioEvent maggotsBurstAudio;

	// Token: 0x04000770 RID: 1904
	private const float UN_MAGGOT_DELAY = 2f;

	// Token: 0x04000771 RID: 1905
	private HeroController insideHero;

	// Token: 0x04000772 RID: 1906
	private Coroutine unMaggotRoutine;

	// Token: 0x04000773 RID: 1907
	private ParticleSystem spawnedHeroMaggots;
}
