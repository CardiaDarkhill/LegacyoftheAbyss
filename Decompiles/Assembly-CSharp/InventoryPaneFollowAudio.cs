using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020006B0 RID: 1712
public class InventoryPaneFollowAudio : MonoBehaviour
{
	// Token: 0x06003D98 RID: 15768 RVA: 0x0010EC68 File Offset: 0x0010CE68
	private void Awake()
	{
		this.paneList.OpeningInventory += delegate()
		{
			if (this.itemCondition && this.itemCondition.GetSavedAmount() <= 0)
			{
				return;
			}
			this.isActive = true;
			this.OnMovedPaneIndex(this.currentPaneIndex, true);
		};
		this.paneList.MovedPaneIndex += delegate(int index)
		{
			this.OnMovedPaneIndex(index, false);
		};
		this.paneList.ClosingInventory += delegate()
		{
			this.isActive = false;
			this.isStopped = false;
			if (this.fadeRoutine != null)
			{
				base.StopCoroutine(this.fadeRoutine);
			}
			this.fadeRoutine = base.StartCoroutine(this.FadeOut());
		};
		if (this.stopAudioEvent)
		{
			this.stopAudioEvent.ReceivedEvent += delegate()
			{
				this.isStopped = true;
				if (this.fadeRoutine != null)
				{
					base.StopCoroutine(this.fadeRoutine);
				}
				this.audioSource.Stop();
			};
		}
		if (this.startAudioEvent)
		{
			this.startAudioEvent.ReceivedEvent += delegate()
			{
				this.isStopped = false;
				this.OnMovedPaneIndex(this.currentPaneIndex, true);
			};
		}
	}

	// Token: 0x06003D99 RID: 15769 RVA: 0x0010ED04 File Offset: 0x0010CF04
	private void OnMovedPaneIndex(int currentIndex, bool isOpening)
	{
		if (this.isStopped)
		{
			return;
		}
		if (!isOpening && this.currentPaneIndex == currentIndex)
		{
			return;
		}
		this.currentPaneIndex = currentIndex;
		if (!this.isActive)
		{
			return;
		}
		int paneIndex = this.paneList.GetPaneIndex(this.mainPane);
		int totalPaneCount = this.paneList.TotalPaneCount;
		AudioClip newClip;
		float lowPassT;
		if (currentIndex == paneIndex)
		{
			newClip = this.mainPaneClip;
			lowPassT = 0f;
		}
		else
		{
			newClip = this.offPaneClip;
			lowPassT = 1f;
		}
		int closestOffsetToIndex = global::Helper.GetClosestOffsetToIndex(paneIndex, currentIndex, totalPaneCount);
		float panStereo;
		float volume;
		if (closestOffsetToIndex <= 1 && closestOffsetToIndex >= -1)
		{
			if (closestOffsetToIndex == 0)
			{
				panStereo = 0f;
				volume = 1f;
			}
			else
			{
				panStereo = (float)((closestOffsetToIndex > 0) ? 1 : -1);
				volume = 1f;
			}
		}
		else
		{
			panStereo = 0f;
			volume = this.backgroundVolume;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeTo(panStereo, newClip, lowPassT, volume, isOpening));
	}

	// Token: 0x06003D9A RID: 15770 RVA: 0x0010EDEE File Offset: 0x0010CFEE
	private IEnumerator FadeTo(float panStereo, AudioClip newClip, float lowPassT, float volume, bool isOpening)
	{
		bool clipChanged = isOpening || this.audioSource.clip != newClip;
		float initialVolume = this.audioSource.volume;
		float initialPanStereo = this.audioSource.panStereo;
		float initialCutoffFreq = this.lowPassFilter.cutoffFrequency;
		float targetCutoffFreq = this.GetCutoffFrequency(lowPassT);
		float fadeUpTime;
		if (isOpening)
		{
			this.audioSource.volume = 0f;
			fadeUpTime = this.openCloseFadeTime;
		}
		else
		{
			for (float elapsed = 0f; elapsed < this.paneFadeTime; elapsed += Time.unscaledDeltaTime)
			{
				float t = elapsed / this.paneFadeTime;
				this.audioSource.panStereo = Mathf.Lerp(initialPanStereo, panStereo, t);
				this.lowPassFilter.cutoffFrequency = Mathf.Lerp(initialCutoffFreq, targetCutoffFreq, t);
				this.audioSource.volume = (clipChanged ? Mathf.Lerp(initialVolume, 0f, t) : Mathf.Lerp(initialVolume, volume, t));
				yield return null;
			}
			fadeUpTime = this.paneFadeTime;
		}
		this.audioSource.panStereo = panStereo;
		this.lowPassFilter.cutoffFrequency = targetCutoffFreq;
		if (!clipChanged)
		{
			yield break;
		}
		this.audioSource.clip = newClip;
		this.audioSource.Play();
		this.audioSource.timeSamples = Random.Range(0, newClip.samples);
		for (float elapsed = 0f; elapsed < fadeUpTime; elapsed += Time.unscaledDeltaTime)
		{
			float t2 = elapsed / fadeUpTime;
			this.audioSource.volume = Mathf.Lerp(0f, volume, t2);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003D9B RID: 15771 RVA: 0x0010EE22 File Offset: 0x0010D022
	private IEnumerator FadeOut()
	{
		float initialVolume = this.audioSource.volume;
		for (float elapsed = 0f; elapsed < this.openCloseFadeTime; elapsed += Time.unscaledDeltaTime)
		{
			float t = elapsed / this.openCloseFadeTime;
			this.audioSource.volume = Mathf.Lerp(initialVolume, 0f, t);
			yield return null;
		}
		this.audioSource.volume = 0f;
		this.audioSource.Stop();
		yield break;
	}

	// Token: 0x06003D9C RID: 15772 RVA: 0x0010EE34 File Offset: 0x0010D034
	private float GetCutoffFrequency(float t)
	{
		float t2 = this.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(t);
		return this.lowPassMapToRange.GetLerpedValue(t2);
	}

	// Token: 0x04003F3F RID: 16191
	[SerializeField]
	private InventoryPaneList paneList;

	// Token: 0x04003F40 RID: 16192
	[SerializeField]
	private InventoryPane mainPane;

	// Token: 0x04003F41 RID: 16193
	[SerializeField]
	private SavedItem itemCondition;

	// Token: 0x04003F42 RID: 16194
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003F43 RID: 16195
	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	// Token: 0x04003F44 RID: 16196
	[SerializeField]
	private MinMaxFloat lowPassMapToRange;

	// Token: 0x04003F45 RID: 16197
	[SerializeField]
	private AudioClip mainPaneClip;

	// Token: 0x04003F46 RID: 16198
	[SerializeField]
	private AudioClip offPaneClip;

	// Token: 0x04003F47 RID: 16199
	[SerializeField]
	private float backgroundVolume;

	// Token: 0x04003F48 RID: 16200
	[SerializeField]
	private float paneFadeTime;

	// Token: 0x04003F49 RID: 16201
	[SerializeField]
	private float openCloseFadeTime;

	// Token: 0x04003F4A RID: 16202
	[SerializeField]
	private EventRegister stopAudioEvent;

	// Token: 0x04003F4B RID: 16203
	[SerializeField]
	private EventRegister startAudioEvent;

	// Token: 0x04003F4C RID: 16204
	private bool isActive;

	// Token: 0x04003F4D RID: 16205
	private int currentPaneIndex;

	// Token: 0x04003F4E RID: 16206
	private bool isStopped;

	// Token: 0x04003F4F RID: 16207
	private Coroutine fadeRoutine;
}
