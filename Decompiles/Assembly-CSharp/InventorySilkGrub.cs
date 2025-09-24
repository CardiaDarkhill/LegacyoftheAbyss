using System;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020006B6 RID: 1718
public class InventorySilkGrub : CustomInventoryItemCollectableDisplay
{
	// Token: 0x06003DEC RID: 15852 RVA: 0x0010FFF9 File Offset: 0x0010E1F9
	private void Awake()
	{
		if (this.enableWhileSelected)
		{
			this.enableWhileSelected.SetActive(false);
		}
	}

	// Token: 0x06003DED RID: 15853 RVA: 0x00110014 File Offset: 0x0010E214
	private void Update()
	{
		if (this.timeUntilBounce > 0f)
		{
			this.timeUntilBounce -= Time.unscaledDeltaTime;
			if (this.timeUntilBounce <= 0f)
			{
				this.PlayBounce();
			}
		}
		if (this.timeUntilSing > 0f)
		{
			this.timeUntilSing -= Time.unscaledDeltaTime;
			if (this.timeUntilSing <= 0f)
			{
				this.animator.Play(InventorySilkGrub._singAnim);
			}
		}
	}

	// Token: 0x06003DEE RID: 15854 RVA: 0x0011008F File Offset: 0x0010E28F
	protected override void OnActivate()
	{
		this.animator.Play(InventorySilkGrub._idleAnim);
		this.StartTimers();
	}

	// Token: 0x06003DEF RID: 15855 RVA: 0x001100A7 File Offset: 0x0010E2A7
	protected override void OnDeactivate()
	{
		this.StopTimers();
	}

	// Token: 0x06003DF0 RID: 15856 RVA: 0x001100B0 File Offset: 0x0010E2B0
	public override void OnSelect()
	{
		this.isSelected = true;
		this.timeUntilBounce = 0.001f;
		if (this.selectedLoop)
		{
			this.selectedLoop.Play();
		}
		if (this.enableWhileSelected)
		{
			this.enableWhileSelected.SetActive(true);
		}
	}

	// Token: 0x06003DF1 RID: 15857 RVA: 0x00110100 File Offset: 0x0010E300
	public override void OnDeselect()
	{
		this.isSelected = false;
		this.timeUntilBounce = this.doBounceTime.GetRandomValue();
		if (this.selectedLoop)
		{
			this.selectedLoop.Stop();
		}
		if (this.enableWhileSelected)
		{
			this.enableWhileSelected.SetActive(false);
		}
	}

	// Token: 0x06003DF2 RID: 15858 RVA: 0x00110158 File Offset: 0x0010E358
	public override void OnConsumeStart()
	{
		PlayerData instance = PlayerData.instance;
		this.hasCocoon = !string.IsNullOrEmpty(instance.HeroCorpseScene);
		bool flag = this.hasCocoon && (instance.HeroCorpseType & HeroDeathCocoonTypes.Cursed) == HeroDeathCocoonTypes.Cursed;
		if (this.hasCocoon)
		{
			this.animator.Play(flag ? InventorySilkGrub._suckCursedAnim : InventorySilkGrub._suckCocoonAnim);
		}
		else
		{
			this.animator.Play(InventorySilkGrub._suckNoCocoonAnim);
		}
		if (this.selectedLoop)
		{
			this.selectedLoop.Stop();
		}
		foreach (AudioSource audioSource in this.chargeLoopSources)
		{
			if (audioSource)
			{
				audioSource.Play();
			}
		}
		this.StopTimers();
	}

	// Token: 0x06003DF3 RID: 15859 RVA: 0x00110214 File Offset: 0x0010E414
	public override void OnConsumeEnd()
	{
		this.ResetToIdle();
		if (this.isSelected && this.selectedLoop)
		{
			this.selectedLoop.Play();
		}
		this.chargeStopAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
	}

	// Token: 0x06003DF4 RID: 15860 RVA: 0x00110264 File Offset: 0x0010E464
	private void ResetToIdle()
	{
		this.animator.Play(InventorySilkGrub._idleAnim);
		foreach (AudioSource audioSource in this.chargeLoopSources)
		{
			if (audioSource)
			{
				audioSource.Stop();
			}
		}
		this.StartTimers();
	}

	// Token: 0x06003DF5 RID: 15861 RVA: 0x001102B0 File Offset: 0x0010E4B0
	public override void OnConsumeComplete()
	{
		this.ResetToIdle();
		if (this.hasCocoon)
		{
			this.consumeCompleteSilkAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			return;
		}
		this.consumeCompleteNoSilkAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
	}

	// Token: 0x06003DF6 RID: 15862 RVA: 0x00110306 File Offset: 0x0010E506
	public override void OnConsumeBlocked()
	{
		this.PlayBounce();
	}

	// Token: 0x06003DF7 RID: 15863 RVA: 0x00110310 File Offset: 0x0010E510
	private void PlayBounce()
	{
		this.timeUntilBounce = (this.isSelected ? this.selectedBounceTime : this.doBounceTime).GetRandomValue();
		this.animator.Play(InventorySilkGrub._bounceAnim);
	}

	// Token: 0x06003DF8 RID: 15864 RVA: 0x00110351 File Offset: 0x0010E551
	private void StartTimers()
	{
		this.timeUntilBounce = this.doBounceTime.GetRandomValue();
		if (SilkGrubCocoon.IsAnyActive)
		{
			this.timeUntilSing = this.doSingTime.GetRandomValue();
		}
	}

	// Token: 0x06003DF9 RID: 15865 RVA: 0x0011037C File Offset: 0x0010E57C
	private void StopTimers()
	{
		this.timeUntilBounce = 0f;
		this.timeUntilSing = 0f;
	}

	// Token: 0x04003F86 RID: 16262
	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	// Token: 0x04003F87 RID: 16263
	private static readonly int _bounceAnim = Animator.StringToHash("Bounce");

	// Token: 0x04003F88 RID: 16264
	private static readonly int _singAnim = Animator.StringToHash("Sing Start");

	// Token: 0x04003F89 RID: 16265
	private static readonly int _suckCocoonAnim = Animator.StringToHash("Suck");

	// Token: 0x04003F8A RID: 16266
	private static readonly int _suckNoCocoonAnim = Animator.StringToHash("Suck No Cocoon");

	// Token: 0x04003F8B RID: 16267
	private static readonly int _suckCursedAnim = Animator.StringToHash("Suck Cursed Start");

	// Token: 0x04003F8C RID: 16268
	[SerializeField]
	private Animator animator;

	// Token: 0x04003F8D RID: 16269
	[SerializeField]
	private MinMaxFloat doBounceTime;

	// Token: 0x04003F8E RID: 16270
	[SerializeField]
	private MinMaxFloat selectedBounceTime;

	// Token: 0x04003F8F RID: 16271
	[SerializeField]
	private MinMaxFloat doSingTime;

	// Token: 0x04003F90 RID: 16272
	[SerializeField]
	private AudioSource selectedLoop;

	// Token: 0x04003F91 RID: 16273
	[SerializeField]
	private AudioSource[] chargeLoopSources;

	// Token: 0x04003F92 RID: 16274
	[SerializeField]
	private AudioEvent chargeStopAudio;

	// Token: 0x04003F93 RID: 16275
	[SerializeField]
	private GameObject enableWhileSelected;

	// Token: 0x04003F94 RID: 16276
	[SerializeField]
	private AudioEvent consumeCompleteSilkAudio;

	// Token: 0x04003F95 RID: 16277
	[SerializeField]
	private AudioEvent consumeCompleteNoSilkAudio;

	// Token: 0x04003F96 RID: 16278
	private bool isSelected;

	// Token: 0x04003F97 RID: 16279
	private float timeUntilBounce;

	// Token: 0x04003F98 RID: 16280
	private float timeUntilSing;

	// Token: 0x04003F99 RID: 16281
	private bool hasCocoon;
}
