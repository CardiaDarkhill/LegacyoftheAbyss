using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000726 RID: 1830
public class SilkChunk : MonoBehaviour
{
	// Token: 0x17000773 RID: 1907
	// (get) Token: 0x06004136 RID: 16694 RVA: 0x0011E3D9 File Offset: 0x0011C5D9
	// (set) Token: 0x06004137 RID: 16695 RVA: 0x0011E3E1 File Offset: 0x0011C5E1
	public bool IsRegen { get; private set; }

	// Token: 0x17000774 RID: 1908
	// (get) Token: 0x06004138 RID: 16696 RVA: 0x0011E3EA File Offset: 0x0011C5EA
	private Color BaseTint
	{
		get
		{
			if (!this.isMaggoted)
			{
				return this.regularTint;
			}
			return this.maggotedColor;
		}
	}

	// Token: 0x06004139 RID: 16697 RVA: 0x0011E401 File Offset: 0x0011C601
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.sprite = base.GetComponent<tk2dSprite>();
		if (this.voidProtectEffect)
		{
			this.voidProtectEffect.SetActive(false);
		}
	}

	// Token: 0x0600413A RID: 16698 RVA: 0x0011E434 File Offset: 0x0011C634
	private void OnEnable()
	{
		this.hc = HeroController.SilentInstance;
		this.gc = GameCameras.SilentInstance;
		this.ResetAppearance();
		this.SetAnims();
	}

	// Token: 0x0600413B RID: 16699 RVA: 0x0011E458 File Offset: 0x0011C658
	private void OnDestroy()
	{
		this.gc = null;
		this.hc = null;
	}

	// Token: 0x0600413C RID: 16700 RVA: 0x0011E468 File Offset: 0x0011C668
	public void Update()
	{
		if (this.removeCounter > 0f)
		{
			this.removeCounter -= Time.deltaTime;
			if (this.removeCounter <= 0f)
			{
				base.gameObject.Recycle();
				return;
			}
		}
		else if (!this.isExtraChunk)
		{
			if (this.isUsing || this.isUsingAcid)
			{
				if (this.queuedUseStart != null && !this.animator.IsPlaying(this.upAnim) && !this.animator.IsPlaying(this.upToGlowAnim))
				{
					this.queuedUseStart();
					this.queuedUseStart = null;
					return;
				}
			}
			else if (!this.IsRegen && !this.animator.IsPlaying(this.upAnim) && !this.animator.IsPlaying(this.upToGlowAnim))
			{
				HeroController heroController = this.hc;
				if (heroController != null && heroController.cState.isMaggoted)
				{
					if (!this.isMaggoted)
					{
						this.isMaggoted = true;
						this.FadeToCorrectColor();
						return;
					}
				}
				else if (this.isMaggoted)
				{
					this.isMaggoted = false;
					this.FadeToCorrectColor();
				}
			}
		}
	}

	// Token: 0x0600413D RID: 16701 RVA: 0x0011E588 File Offset: 0x0011C788
	private void FadeToCorrectColor()
	{
		Color color;
		if (this.isMaggoted)
		{
			color = (this.isGlowing ? Color.Lerp(this.maggotedColor, this.regularTint, this.bindableColourLerp) : this.maggotedColor);
		}
		else
		{
			color = this.regularTint;
		}
		this.FadeToColor(0f, this.maggotFadeTime, color);
	}

	// Token: 0x0600413E RID: 16702 RVA: 0x0011E5E0 File Offset: 0x0011C7E0
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.ResetAppearance();
	}

	// Token: 0x0600413F RID: 16703 RVA: 0x0011E5F0 File Offset: 0x0011C7F0
	public void Add(bool glowing)
	{
		this.IsRegen = false;
		if (this.TryIsBigSilk())
		{
			this.isGlowing = true;
		}
		else if (glowing)
		{
			this.animator.Play(this.upToGlowAnim);
			this.isGlowing = true;
		}
		else
		{
			this.animator.Play(this.upAnim);
			this.isGlowing = false;
		}
		this.ResetAppearance();
	}

	// Token: 0x06004140 RID: 16704 RVA: 0x0011E650 File Offset: 0x0011C850
	private bool TryIsBigSilk()
	{
		if (PlayerData.instance.UnlockSilkFinalCutscene)
		{
			this.animator.PlayFromFrame(this.silkDefeatAnim, 0);
			return true;
		}
		return false;
	}

	// Token: 0x06004141 RID: 16705 RVA: 0x0011E674 File Offset: 0x0011C874
	private void ResetAppearance()
	{
		this.removeCounter = 0f;
		if (this.mossAppearEffect)
		{
			this.mossAppearEffect.SetActive(false);
		}
		if (this.mossAppearEffectHalf)
		{
			this.mossAppearEffectHalf.SetActive(false);
		}
		if (this.drainEffect)
		{
			this.drainEffect.StopParticleSystems();
		}
		this.previousPartsAnimIndex = -1;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		this.StopColorFade();
		this.isExtraChunk = false;
		this.isMaggoted = false;
		this.sprite.color = this.BaseTint;
		this.queuedUseStart = null;
	}

	// Token: 0x06004142 RID: 16706 RVA: 0x0011E728 File Offset: 0x0011C928
	public void SetUsing(SilkSpool.SilkUsingFlags usingFlags)
	{
		this.ResetAppearance();
		this.isUsing = true;
		if ((usingFlags & (SilkSpool.SilkUsingFlags.Normal | SilkSpool.SilkUsingFlags.Maggot)) == (SilkSpool.SilkUsingFlags.Normal | SilkSpool.SilkUsingFlags.Maggot))
		{
			this.animator.Play("SilkChunk Temp Use");
			this.sprite.color = this.maggotUseColor;
			return;
		}
		if ((usingFlags & SilkSpool.SilkUsingFlags.Drain) != SilkSpool.SilkUsingFlags.None)
		{
			this.animator.Play("SilkChunk Temp Use");
			if (this.drainEffect)
			{
				this.drainEffect.PlayParticleSystems();
				return;
			}
		}
		else
		{
			if ((usingFlags & SilkSpool.SilkUsingFlags.Normal) != SilkSpool.SilkUsingFlags.None)
			{
				this.animator.Play("SilkChunk Temp Use");
				return;
			}
			if ((usingFlags & SilkSpool.SilkUsingFlags.Acid) != SilkSpool.SilkUsingFlags.None)
			{
				this.isUsingAcid = true;
				this.queuedUseStart = delegate()
				{
					this.animator.Play(this.acidUseAnim);
					this.sprite.color = this.acidUseColor;
					if (this.acidUseEffect)
					{
						this.acidUseEffect.PlayParticleSystems();
					}
				};
				return;
			}
			if ((usingFlags & SilkSpool.SilkUsingFlags.Maggot) != SilkSpool.SilkUsingFlags.None)
			{
				this.isUsingAcid = true;
				this.queuedUseStart = delegate()
				{
					this.animator.Play(this.acidUseAnim);
					this.StopColorFade();
					this.sprite.color = this.maggotUseColor;
					if (this.maggotUseEffect)
					{
						this.maggotUseEffect.PlayParticleSystems();
					}
				};
				return;
			}
			if ((usingFlags & SilkSpool.SilkUsingFlags.Void) != SilkSpool.SilkUsingFlags.None)
			{
				this.isUsingAcid = true;
				this.queuedUseStart = delegate()
				{
					this.animator.Play(this.voidUseAnim);
					this.sprite.color = this.regularTint;
					if (this.voidUseEffect)
					{
						this.voidUseEffect.PlayParticleSystems();
					}
				};
				return;
			}
			if ((usingFlags & SilkSpool.SilkUsingFlags.Curse) != SilkSpool.SilkUsingFlags.None)
			{
				this.animator.Play(this.cursedUseAnims[Random.Range(0, this.cursedUseAnims.Length)]);
			}
		}
	}

	// Token: 0x06004143 RID: 16707 RVA: 0x0011E835 File Offset: 0x0011CA35
	public void SetRegen(bool isUpgraded)
	{
		this.animator.Play("SilkChunk Temp Get");
		this.IsRegen = true;
		if (this.gc.IsHudVisible)
		{
			this.regeneratingAudioLoop.Play();
		}
		this.ResetAppearance();
	}

	// Token: 0x06004144 RID: 16708 RVA: 0x0011E86C File Offset: 0x0011CA6C
	public void ResumeRegenAudioLoop()
	{
		if (!this.IsRegen || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.gc.IsHudVisible)
		{
			this.regeneratingAudioLoop.Play();
		}
	}

	// Token: 0x06004145 RID: 16709 RVA: 0x0011E89C File Offset: 0x0011CA9C
	public void StopRegenAudioLoop()
	{
		this.regeneratingAudioLoop.Stop();
	}

	// Token: 0x06004146 RID: 16710 RVA: 0x0011E8AC File Offset: 0x0011CAAC
	public void EndedRegen()
	{
		GameCameras instance = GameCameras.instance;
		if (instance != null && !instance.IsHudVisible)
		{
			return;
		}
		this.regeneratedSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
	}

	// Token: 0x06004147 RID: 16711 RVA: 0x0011E8F0 File Offset: 0x0011CAF0
	public void Remove(SilkSpool.SilkTakeSource takeSource)
	{
		switch (takeSource)
		{
		case SilkSpool.SilkTakeSource.Normal:
			this.animator.Play(this.isUsingAcid ? this.downAcidAnim : this.downAnim);
			this.removeCounter = 0.135f;
			return;
		case SilkSpool.SilkTakeSource.Wisp:
			this.animator.Play(this.downAnim);
			this.wispUseEffect.PlayParticleSystems();
			this.removeCounter = 0.85f;
			return;
		case SilkSpool.SilkTakeSource.Curse:
			this.animator.Play("SilkChunk Gone");
			this.cursedBurstEffect.PlayParticleSystems();
			this.removeCounter = 1.1f;
			return;
		case SilkSpool.SilkTakeSource.Drain:
			this.animator.Play(this.downAnim);
			this.removeCounter = 0.135f;
			if (this.gc.IsHudVisible)
			{
				this.takenCutSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
				return;
			}
			break;
		case SilkSpool.SilkTakeSource.ActiveUse:
			this.animator.Play(this.isUsingAcid ? this.downAcidAnim : this.downAnim);
			this.removeCounter = 0.135f;
			if (this.gc.IsHudVisible)
			{
				this.regeneratedSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
				return;
			}
			break;
		case SilkSpool.SilkTakeSource.Parts:
			this.animator.Play("SilkChunk Pilgrim Disperse");
			this.removeCounter = 0.15f;
			return;
		default:
			throw new ArgumentOutOfRangeException("takeSource", takeSource, null);
		}
	}

	// Token: 0x06004148 RID: 16712 RVA: 0x0011EA6B File Offset: 0x0011CC6B
	public void StartGlow()
	{
		if (this.TryIsBigSilk())
		{
			return;
		}
		if (this.isGlowing)
		{
			return;
		}
		this.isGlowing = true;
		this.PlayIdle();
		this.FadeToCorrectColor();
	}

	// Token: 0x06004149 RID: 16713 RVA: 0x0011EA92 File Offset: 0x0011CC92
	public void EndGlow()
	{
		if (this.TryIsBigSilk())
		{
			return;
		}
		if (!this.isGlowing)
		{
			return;
		}
		this.isGlowing = false;
		this.PlayIdle();
		this.FadeToCorrectColor();
	}

	// Token: 0x0600414A RID: 16714 RVA: 0x0011EABC File Offset: 0x0011CCBC
	public void PlayIdle()
	{
		this.FadeToColor(0f, this.acidFadeBackTime, this.BaseTint);
		this.animator.Play(this.isGlowing ? this.glowAnim : this.idleAnim);
		if (!this.isUsing)
		{
			return;
		}
		this.isUsing = false;
		this.isUsingAcid = false;
		if (this.acidUseEffect)
		{
			this.acidUseEffect.StopParticleSystems();
		}
		if (this.maggotUseEffect)
		{
			this.maggotUseEffect.StopParticleSystems();
		}
		if (this.voidUseEffect && this.voidUseEffect.IsPlaying())
		{
			this.voidUseEffect.StopParticleSystems();
			if (this.voidProtectEffect && this.hc && this.hc.playerData.HasWhiteFlower)
			{
				this.voidProtectEffect.SetActive(true);
			}
		}
		if (this.drainEffect)
		{
			this.drainEffect.StopParticleSystems();
		}
	}

	// Token: 0x0600414B RID: 16715 RVA: 0x0011EBC0 File Offset: 0x0011CDC0
	private void SetAnims()
	{
		int num = Random.Range(1, 90);
		if (num < 30)
		{
			this.upToGlowAnim = "SilkChunk UpToGlow1";
			this.upAnim = "SilkChunk Up1";
			this.downAnim = "SilkChunk Down1";
			this.downAcidAnim = "SilkChunk Down Acid1";
			this.idleAnim = "SilkChunk Idle 1";
			this.glowAnim = "SilkChunk Glow 1";
			return;
		}
		if (num < 60)
		{
			this.upToGlowAnim = "SilkChunk UpToGlow2";
			this.upAnim = "SilkChunk Up2";
			this.downAnim = "SilkChunk Down2";
			this.downAcidAnim = "SilkChunk Down Acid2";
			this.idleAnim = "SilkChunk Idle 2";
			this.glowAnim = "SilkChunk Glow 2";
			return;
		}
		this.upToGlowAnim = "SilkChunk UpToGlow3";
		this.upAnim = "SilkChunk Up3";
		this.downAnim = "SilkChunk Down3";
		this.downAcidAnim = "SilkChunk Down Acid3";
		this.idleAnim = "SilkChunk Idle 3";
		this.glowAnim = "SilkChunk Glow 3";
	}

	// Token: 0x0600414C RID: 16716 RVA: 0x0011ECA8 File Offset: 0x0011CEA8
	public void SetMossState(int index)
	{
		this.sprite.color = this.mossTint;
		if (index >= 0)
		{
			if (index < this.mossAnims.Length)
			{
				this.animator.Play(this.mossAnims[index]);
			}
			if (index < this.mossAudio.Length)
			{
				this.mossAudio[index].SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			}
		}
		this.isExtraChunk = true;
		if (this.mossAppearEffectHalf)
		{
			this.mossAppearEffectHalf.SetActive(true);
		}
	}

	// Token: 0x0600414D RID: 16717 RVA: 0x0011ED38 File Offset: 0x0011CF38
	public void FinishMossState()
	{
		float delay = 0f;
		if (this.mossAnims.Length != 0)
		{
			tk2dSpriteAnimator self = this.animator;
			string[] array = this.mossAnims;
			delay = self.PlayAnimGetTime(array[array.Length - 1]);
		}
		if (this.mossAudio.Length != 0)
		{
			AudioEvent[] array2 = this.mossAudio;
			array2[array2.Length - 1].SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		}
		this.FadeToColor(delay, this.mossTintFadeTime, this.BaseTint);
		if (this.mossAppearEffect)
		{
			this.mossAppearEffect.SetActive(true);
		}
		this.isExtraChunk = false;
	}

	// Token: 0x0600414E RID: 16718 RVA: 0x0011EDD0 File Offset: 0x0011CFD0
	private void FadeToColor(float delay, float fadeTime, Color color)
	{
		this.StopColorFade();
		this.fadeStartColor = this.sprite.color;
		this.fadeTargetColor = color;
		if (!base.isActiveAndEnabled)
		{
			this.sprite.color = this.fadeTargetColor;
			return;
		}
		this.colorFadeRoutine = this.StartTimerRoutine(delay, fadeTime, delegate(float t)
		{
			this.sprite.color = Color.Lerp(this.fadeStartColor, this.fadeTargetColor, t);
		}, delegate
		{
			this.animator.Play(this.isGlowing ? this.glowAnim : this.idleAnim);
		}, null, false);
	}

	// Token: 0x0600414F RID: 16719 RVA: 0x0011EE40 File Offset: 0x0011D040
	public void SetPartsState(int index)
	{
		this.sprite.color = this.BaseTint;
		if (index == this.previousPartsAnimIndex || index < 0 || index >= this.pilgrimAnims.Length)
		{
			return;
		}
		this.animator.Play(this.pilgrimAnims[index]);
		this.previousPartsAnimIndex = index;
		this.isExtraChunk = true;
	}

	// Token: 0x06004150 RID: 16720 RVA: 0x0011EE98 File Offset: 0x0011D098
	public void FinishPartsState()
	{
		int num = this.pilgrimAnims.Length;
		if (num > 0)
		{
			this.animator.Play(this.pilgrimAnims[num - 1]);
		}
		this.previousPartsAnimIndex = -1;
		this.isExtraChunk = false;
	}

	// Token: 0x06004151 RID: 16721 RVA: 0x0011EED5 File Offset: 0x0011D0D5
	private void StopColorFade()
	{
		if (this.colorFadeRoutine != null)
		{
			base.StopCoroutine(this.colorFadeRoutine);
		}
	}

	// Token: 0x04004299 RID: 17049
	[SerializeField]
	private AudioSource regeneratingAudioLoop;

	// Token: 0x0400429A RID: 17050
	[SerializeField]
	private AudioEvent regeneratedSound;

	// Token: 0x0400429B RID: 17051
	[SerializeField]
	private AudioEvent takenCutSound;

	// Token: 0x0400429C RID: 17052
	[Header("Regular Appear")]
	[SerializeField]
	private Color regularTint;

	// Token: 0x0400429D RID: 17053
	[Header("Moss Appear")]
	[SerializeField]
	private Color mossTint;

	// Token: 0x0400429E RID: 17054
	[SerializeField]
	private float mossTintFadeTime;

	// Token: 0x0400429F RID: 17055
	[SerializeField]
	private GameObject mossAppearEffect;

	// Token: 0x040042A0 RID: 17056
	[SerializeField]
	private GameObject mossAppearEffectHalf;

	// Token: 0x040042A1 RID: 17057
	[SerializeField]
	private string[] mossAnims;

	// Token: 0x040042A2 RID: 17058
	[SerializeField]
	private AudioEvent[] mossAudio;

	// Token: 0x040042A3 RID: 17059
	[Header("Pilgrim Appear")]
	[SerializeField]
	private string[] pilgrimAnims;

	// Token: 0x040042A4 RID: 17060
	[Header("Acid Use")]
	[SerializeField]
	private string acidUseAnim;

	// Token: 0x040042A5 RID: 17061
	[SerializeField]
	private Color acidUseColor;

	// Token: 0x040042A6 RID: 17062
	[SerializeField]
	private PlayParticleEffects acidUseEffect;

	// Token: 0x040042A7 RID: 17063
	[SerializeField]
	private float acidFadeBackTime;

	// Token: 0x040042A8 RID: 17064
	[Space]
	[SerializeField]
	private Color maggotedColor;

	// Token: 0x040042A9 RID: 17065
	[SerializeField]
	[Range(0f, 1f)]
	private float bindableColourLerp = 0.3f;

	// Token: 0x040042AA RID: 17066
	[SerializeField]
	private Color maggotUseColor;

	// Token: 0x040042AB RID: 17067
	[SerializeField]
	private float maggotFadeTime;

	// Token: 0x040042AC RID: 17068
	[SerializeField]
	private PlayParticleEffects maggotUseEffect;

	// Token: 0x040042AD RID: 17069
	[Space]
	[SerializeField]
	private string voidUseAnim;

	// Token: 0x040042AE RID: 17070
	[SerializeField]
	private PlayParticleEffects voidUseEffect;

	// Token: 0x040042AF RID: 17071
	[SerializeField]
	private GameObject voidProtectEffect;

	// Token: 0x040042B0 RID: 17072
	[Header("Cursed")]
	[SerializeField]
	private string[] cursedUseAnims;

	// Token: 0x040042B1 RID: 17073
	[SerializeField]
	private PlayParticleEffects cursedBurstEffect;

	// Token: 0x040042B2 RID: 17074
	[Header("Silk Defeat Death")]
	[SerializeField]
	private string silkDefeatAnim;

	// Token: 0x040042B3 RID: 17075
	[Header("Take Effects")]
	[SerializeField]
	private PlayParticleEffects wispUseEffect;

	// Token: 0x040042B4 RID: 17076
	[SerializeField]
	private PlayParticleEffects drainEffect;

	// Token: 0x040042B5 RID: 17077
	private tk2dSpriteAnimator animator;

	// Token: 0x040042B6 RID: 17078
	private tk2dSprite sprite;

	// Token: 0x040042B7 RID: 17079
	private float removeCounter;

	// Token: 0x040042B8 RID: 17080
	private Coroutine colorFadeRoutine;

	// Token: 0x040042B9 RID: 17081
	private bool waitForAcidParticles;

	// Token: 0x040042BA RID: 17082
	private bool isExtraChunk;

	// Token: 0x040042BB RID: 17083
	private bool isGlowing;

	// Token: 0x040042BC RID: 17084
	private bool isUsing;

	// Token: 0x040042BD RID: 17085
	private bool isUsingAcid;

	// Token: 0x040042BE RID: 17086
	private bool isMaggoted;

	// Token: 0x040042BF RID: 17087
	private string upToGlowAnim;

	// Token: 0x040042C0 RID: 17088
	private string upAnim;

	// Token: 0x040042C1 RID: 17089
	private string downAnim;

	// Token: 0x040042C2 RID: 17090
	private string downAcidAnim;

	// Token: 0x040042C3 RID: 17091
	private string idleAnim;

	// Token: 0x040042C4 RID: 17092
	private string glowAnim;

	// Token: 0x040042C5 RID: 17093
	private int previousPartsAnimIndex;

	// Token: 0x040042C6 RID: 17094
	private Color fadeStartColor;

	// Token: 0x040042C7 RID: 17095
	private Color fadeTargetColor;

	// Token: 0x040042C8 RID: 17096
	private Action queuedUseStart;

	// Token: 0x040042C9 RID: 17097
	private HeroController hc;

	// Token: 0x040042CA RID: 17098
	private GameCameras gc;
}
