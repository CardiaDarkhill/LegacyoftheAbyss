using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000586 RID: 1414
public class WeaverHeatLamp : MonoBehaviour
{
	// Token: 0x06003292 RID: 12946 RVA: 0x000E120E File Offset: 0x000DF40E
	private void OnEnable()
	{
		HeroController.instance.OnHazardRespawn += this.OnHeroHazardRespawn;
		if (this.appearTrigger)
		{
			this.appearTrigger.InsideStateChanged += this.OnAppearTriggerState;
		}
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x000E124C File Offset: 0x000DF44C
	private void Start()
	{
		this.warmthRegion.SetBool(WeaverHeatLamp._activeId, false);
		this.warmthRegion.Play(WeaverHeatLamp._disappearId, 0, 1f);
		this.core.SetBool(WeaverHeatLamp._activeId, false);
		this.core.Play(WeaverHeatLamp._disappearId, 0, 1f);
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x000E12A8 File Offset: 0x000DF4A8
	private void OnDisable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance)
		{
			silentInstance.OnHazardRespawn -= this.OnHeroHazardRespawn;
		}
		if (this.appearTrigger)
		{
			this.appearTrigger.InsideStateChanged -= this.OnAppearTriggerState;
		}
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x000E12F9 File Offset: 0x000DF4F9
	public void RefreshUnlocked()
	{
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x000E12FC File Offset: 0x000DF4FC
	private void OnAppearTriggerState(bool isInside)
	{
		this.isInside = isInside;
		this.warmthRegion.SetBool(WeaverHeatLamp._activeId, isInside);
		this.core.SetBool(WeaverHeatLamp._activeId, isInside);
		if (isInside)
		{
			this.Activated();
			if (this.insideAudioLoop)
			{
				this.insideAudioLoop.Play();
				return;
			}
		}
		else
		{
			if (base.gameObject.scene.isLoaded)
			{
				this.exitSound.SpawnAndPlayOneShot(base.transform.position, null);
			}
			if (this.insideAudioLoop)
			{
				this.insideAudioLoop.Stop();
			}
		}
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x000E1399 File Offset: 0x000DF599
	public void HitActivate()
	{
		this.Activated();
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x000E13A4 File Offset: 0x000DF5A4
	private void Activated()
	{
		this.warmthRegion.SetTrigger(WeaverHeatLamp._activateId);
		this.core.SetTrigger(WeaverHeatLamp._activateId);
		if (this.snowFadeOffRoutine == null && this.snowFader && this.snowFader.AlphaSelf > Mathf.Epsilon)
		{
			this.snowFadeOffRoutine = base.StartCoroutine(this.SnowFadeOff());
		}
		this.enterSound.SpawnAndPlayOneShot(base.transform.position, null);
	}

	// Token: 0x06003299 RID: 12953 RVA: 0x000E1422 File Offset: 0x000DF622
	private IEnumerator SnowFadeOff()
	{
		float initialAlpha = this.snowFader.AlphaSelf;
		for (float elapsed = 0f; elapsed <= this.snowFadeOffDuration; elapsed += Time.deltaTime)
		{
			float t = this.snowFadeOffCurve.Evaluate(elapsed / this.snowFadeOffDuration);
			this.snowFader.AlphaSelf = Mathf.Lerp(0f, initialAlpha, t);
			yield return null;
		}
		this.snowFader.AlphaSelf = 0f;
		yield break;
	}

	// Token: 0x0600329A RID: 12954 RVA: 0x000E1434 File Offset: 0x000DF634
	private void OnHeroHazardRespawn()
	{
		if (this.isInside)
		{
			return;
		}
		this.core.SetBool(WeaverHeatLamp._activeId, false);
		this.core.Play(WeaverHeatLamp._disappearId, 0, 1f);
		this.warmthRegion.Play(WeaverHeatLamp._disappearId, 0, 1f);
	}

	// Token: 0x04003664 RID: 13924
	[SerializeField]
	private TrackTriggerObjects appearTrigger;

	// Token: 0x04003665 RID: 13925
	[Space]
	[SerializeField]
	private Animator core;

	// Token: 0x04003666 RID: 13926
	[SerializeField]
	private Animator warmthRegion;

	// Token: 0x04003667 RID: 13927
	[Space]
	[SerializeField]
	private NestedFadeGroupBase snowFader;

	// Token: 0x04003668 RID: 13928
	[SerializeField]
	private float snowFadeOffDuration;

	// Token: 0x04003669 RID: 13929
	[SerializeField]
	private AnimationCurve snowFadeOffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x0400366A RID: 13930
	[Space]
	[SerializeField]
	private AudioSource insideAudioLoop;

	// Token: 0x0400366B RID: 13931
	[SerializeField]
	private AudioEvent enterSound;

	// Token: 0x0400366C RID: 13932
	[SerializeField]
	private AudioEvent exitSound;

	// Token: 0x0400366D RID: 13933
	private Coroutine snowFadeOffRoutine;

	// Token: 0x0400366E RID: 13934
	private bool isInside;

	// Token: 0x0400366F RID: 13935
	private static readonly int _activateId = Animator.StringToHash("Activate");

	// Token: 0x04003670 RID: 13936
	private static readonly int _activeId = Animator.StringToHash("Active");

	// Token: 0x04003671 RID: 13937
	private static readonly int _disappearId = Animator.StringToHash("Disappear");
}
