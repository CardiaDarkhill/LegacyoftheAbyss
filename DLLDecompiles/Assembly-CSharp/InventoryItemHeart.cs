using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200068F RID: 1679
public class InventoryItemHeart : CustomInventoryItemCollectableDisplay
{
	// Token: 0x06003BF3 RID: 15347 RVA: 0x00108054 File Offset: 0x00106254
	private void Awake()
	{
		this.hasAudioSource = (this.audioSource != null);
		if (!this.hasAudioSource)
		{
			this.audioSource = base.GetComponentInChildren<AudioSource>();
			this.hasAudioSource = (this.audioSource != null);
		}
		if (this.hasAudioSource)
		{
			this.originalVolume = this.audioSource.volume;
		}
	}

	// Token: 0x06003BF4 RID: 15348 RVA: 0x001080B4 File Offset: 0x001062B4
	private void Update()
	{
		bool flag = InventoryItemHeart.TickChill(ref this.timeUntilChillBeat, this.unselectedBeatDelay);
		bool flag2 = InventoryItemHeart.TickChill(ref this.timeUntilFastBeat, this.selectedBeatDelay);
		if (this.isSelected)
		{
			if (!flag2)
			{
				return;
			}
			if (this.lowPassFilter)
			{
				this.lowPassFilter.cutoffFrequency = this.selectedCutoff;
			}
			this.OnBeatSelected.Invoke();
		}
		else
		{
			if (!flag)
			{
				return;
			}
			if (this.lowPassFilter)
			{
				this.lowPassFilter.cutoffFrequency = this.unselectedCutoff;
			}
			this.OnBeatUnselected.Invoke();
		}
		this.animator.Play(InventoryItemHeart._beatAnim);
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x00108159 File Offset: 0x00106359
	private static bool TickChill(ref float timeUntilBeat, MinMaxFloat resetVal)
	{
		if (timeUntilBeat <= 0f)
		{
			return false;
		}
		timeUntilBeat -= Time.unscaledDeltaTime;
		if (timeUntilBeat > 0f)
		{
			return false;
		}
		timeUntilBeat = resetVal.GetRandomValue();
		return true;
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x00108188 File Offset: 0x00106388
	protected override void OnActivate()
	{
		this.animator.Play(InventoryItemHeart._idleAnim);
		this.timeUntilChillBeat = this.unselectedBeatDelay.GetRandomValue() + this.startOffsetDelay.GetRandomValue();
		this.timeUntilFastBeat = 0f;
		this.isSelected = false;
		this.StopAudio();
	}

	// Token: 0x06003BF7 RID: 15351 RVA: 0x001081DA File Offset: 0x001063DA
	protected override void OnDeactivate()
	{
		this.timeUntilChillBeat = 0f;
		this.timeUntilFastBeat = 0f;
		this.isSelected = false;
		this.StopAudio();
	}

	// Token: 0x06003BF8 RID: 15352 RVA: 0x001081FF File Offset: 0x001063FF
	public override void OnSelect()
	{
		this.timeUntilFastBeat = 0.001f;
		this.isSelected = true;
		this.PlayAudio();
	}

	// Token: 0x06003BF9 RID: 15353 RVA: 0x00108219 File Offset: 0x00106419
	public override void OnDeselect()
	{
		this.timeUntilFastBeat = 0f;
		this.isSelected = false;
		this.StopAudio();
	}

	// Token: 0x06003BFA RID: 15354 RVA: 0x00108233 File Offset: 0x00106433
	private void PlayAudio()
	{
		if (!this.hasAudioSource)
		{
			return;
		}
		if (this.handleAudioPlayStop)
		{
			this.audioSource.Play();
		}
		if (this.updateVolume)
		{
			this.audioSource.volume = this.originalVolume;
		}
	}

	// Token: 0x06003BFB RID: 15355 RVA: 0x0010826A File Offset: 0x0010646A
	private void StopAudio()
	{
		if (!this.hasAudioSource)
		{
			return;
		}
		if (!this.handleAudioPlayStop)
		{
			this.audioSource.Stop();
		}
		if (this.updateVolume)
		{
			this.audioSource.volume = 0f;
		}
	}

	// Token: 0x04003E0F RID: 15887
	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	// Token: 0x04003E10 RID: 15888
	private static readonly int _beatAnim = Animator.StringToHash("Beat");

	// Token: 0x04003E11 RID: 15889
	[SerializeField]
	private Animator animator;

	// Token: 0x04003E12 RID: 15890
	[SerializeField]
	private MinMaxFloat startOffsetDelay;

	// Token: 0x04003E13 RID: 15891
	[SerializeField]
	private MinMaxFloat unselectedBeatDelay;

	// Token: 0x04003E14 RID: 15892
	[SerializeField]
	private MinMaxFloat selectedBeatDelay;

	// Token: 0x04003E15 RID: 15893
	[Space]
	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	// Token: 0x04003E16 RID: 15894
	[SerializeField]
	private float selectedCutoff;

	// Token: 0x04003E17 RID: 15895
	[SerializeField]
	private float unselectedCutoff;

	// Token: 0x04003E18 RID: 15896
	[Space]
	public UnityEvent OnBeatUnselected;

	// Token: 0x04003E19 RID: 15897
	public UnityEvent OnBeatSelected;

	// Token: 0x04003E1A RID: 15898
	[SerializeField]
	private bool handleAudioPlayStop;

	// Token: 0x04003E1B RID: 15899
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003E1C RID: 15900
	[SerializeField]
	private bool updateVolume;

	// Token: 0x04003E1D RID: 15901
	private bool isSelected;

	// Token: 0x04003E1E RID: 15902
	private float timeUntilChillBeat;

	// Token: 0x04003E1F RID: 15903
	private float timeUntilFastBeat;

	// Token: 0x04003E20 RID: 15904
	private bool hasAudioSource;

	// Token: 0x04003E21 RID: 15905
	private float originalVolume;
}
