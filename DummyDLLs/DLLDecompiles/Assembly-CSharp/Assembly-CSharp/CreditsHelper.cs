using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000078 RID: 120
public class CreditsHelper : MonoBehaviour
{
	// Token: 0x06000368 RID: 872 RVA: 0x00011CC0 File Offset: 0x0000FEC0
	private void Awake()
	{
		this.creditsSections = new List<CreditsSectionBase>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			CreditsSectionBase component = base.transform.GetChild(i).GetComponent<CreditsSectionBase>();
			if (component)
			{
				this.creditsSections.Add(component);
			}
		}
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00011D20 File Offset: 0x0000FF20
	private void Start()
	{
		foreach (CreditsSectionBase creditsSectionBase in this.creditsSections)
		{
			creditsSectionBase.gameObject.SetActive(false);
		}
		if (this.activateOnEnd)
		{
			this.activateOnEnd.SetActive(false);
		}
		GameCameras.instance.cameraController.IsBloomForced = true;
		base.StartCoroutine(this.Sequence());
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00011DAC File Offset: 0x0000FFAC
	private IEnumerator Sequence()
	{
		this.screenFader.AlphaSelf = 1f;
		if (this.silentSnapshot != null)
		{
			this.silentSnapshot.TransitionTo(0f);
		}
		yield return new WaitForSeconds(this.startPause);
		this.musicSource.Play();
		int num;
		for (int i = 0; i < this.creditsSections.Count; i = num + 1)
		{
			CreditsSectionBase creditsSection = this.creditsSections[i];
			creditsSection.gameObject.SetActive(true);
			if (i == 0 && this.musicSnapshot != null)
			{
				this.musicSnapshot.TransitionTo(creditsSection.FadeUpDuration);
			}
			yield return new WaitForSeconds(this.screenFader.FadeTo(0f, creditsSection.FadeUpDuration, null, false, null));
			yield return creditsSection.Show();
			if (i >= this.creditsSections.Count - 1 && this.silentSnapshot != null)
			{
				this.silentSnapshot.TransitionTo(creditsSection.FadeDownDuration + this.timeBetweenScreens);
			}
			yield return new WaitForSeconds(this.screenFader.FadeTo(1f, creditsSection.FadeDownDuration, null, false, null));
			creditsSection.gameObject.SetActive(false);
			yield return new WaitForSeconds(this.timeBetweenScreens);
			creditsSection = null;
			num = i;
		}
		yield return new WaitForSeconds(this.endPause);
		base.StartCoroutine(this.cutSceneHelper.Skip());
		GameCameras.instance.cameraController.IsBloomForced = false;
		yield break;
	}

	// Token: 0x0400030E RID: 782
	[SerializeField]
	private CutsceneHelper cutSceneHelper;

	// Token: 0x0400030F RID: 783
	[SerializeField]
	private NestedFadeGroupBase screenFader;

	// Token: 0x04000310 RID: 784
	[SerializeField]
	private float startPause;

	// Token: 0x04000311 RID: 785
	[SerializeField]
	private float timeBetweenScreens;

	// Token: 0x04000312 RID: 786
	[SerializeField]
	private float endPause;

	// Token: 0x04000313 RID: 787
	[SerializeField]
	private GameObject activateOnEnd;

	// Token: 0x04000314 RID: 788
	[Space]
	[SerializeField]
	private AudioSource musicSource;

	// Token: 0x04000315 RID: 789
	[SerializeField]
	private AudioMixerSnapshot silentSnapshot;

	// Token: 0x04000316 RID: 790
	[SerializeField]
	private AudioMixerSnapshot musicSnapshot;

	// Token: 0x04000317 RID: 791
	private List<CreditsSectionBase> creditsSections;
}
