using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020000F9 RID: 249
public class AtmosRegion : TrackTriggerObjects
{
	// Token: 0x060007D3 RID: 2003 RVA: 0x000257B1 File Offset: 0x000239B1
	protected override void Awake()
	{
		base.Awake();
		if (base.gameObject.layer != 13)
		{
			base.gameObject.layer = 13;
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x000257D8 File Offset: 0x000239D8
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside)
		{
			if (this.fadeInRoutine != null)
			{
				base.StopCoroutine(this.fadeInRoutine);
			}
			this.fadeInRoutine = base.StartCoroutine(this.FadeIn());
			return;
		}
		if (this.fadeInRoutine != null)
		{
			base.StopCoroutine(this.fadeInRoutine);
		}
		this.FadeOut();
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00025829 File Offset: 0x00023A29
	private IEnumerator FadeIn()
	{
		if (this.delay > 0)
		{
			yield return new WaitForSeconds((float)this.delay);
		}
		GameManager instance = GameManager.instance;
		if (this.enterAtmosSnapshot != null)
		{
			AudioManager.TransitionToAtmosOverride(this.enterAtmosSnapshot, this.enterTransitionTime);
		}
		if (this.enterAtmosCue)
		{
			instance.AudioManager.ApplyAtmosCue(this.enterAtmosCue, this.enterTransitionTime);
		}
		this.fadeInRoutine = null;
		yield break;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00025838 File Offset: 0x00023A38
	private void FadeOut()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		if (this.exitAtmosSnapshot != null)
		{
			AudioManager.TransitionToAtmosOverride(this.enterAtmosSnapshot, this.exitTransitionTime);
		}
		if (this.exitAtmosCue)
		{
			silentInstance.AudioManager.ApplyAtmosCue(this.exitAtmosCue, this.exitTransitionTime);
		}
	}

	// Token: 0x04000793 RID: 1939
	[Space]
	[SerializeField]
	private int delay;

	// Token: 0x04000794 RID: 1940
	[SerializeField]
	private AtmosCue enterAtmosCue;

	// Token: 0x04000795 RID: 1941
	[SerializeField]
	private AudioMixerSnapshot enterAtmosSnapshot;

	// Token: 0x04000796 RID: 1942
	[SerializeField]
	private float enterTransitionTime;

	// Token: 0x04000797 RID: 1943
	[Space]
	[SerializeField]
	private AtmosCue exitAtmosCue;

	// Token: 0x04000798 RID: 1944
	[SerializeField]
	private AudioMixerSnapshot exitAtmosSnapshot;

	// Token: 0x04000799 RID: 1945
	[SerializeField]
	private float exitTransitionTime;

	// Token: 0x0400079A RID: 1946
	private Coroutine fadeInRoutine;
}
