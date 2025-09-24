using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000650 RID: 1616
public class Remasker : MaskerBase, ScenePrefabInstanceFix.ICheckFields
{
	// Token: 0x1700068F RID: 1679
	// (get) Token: 0x060039CB RID: 14795 RVA: 0x000FD6A3 File Offset: 0x000FB8A3
	// (set) Token: 0x060039CC RID: 14796 RVA: 0x000FD6AB File Offset: 0x000FB8AB
	public bool ForceUncovered { get; set; }

	// Token: 0x17000690 RID: 1680
	// (get) Token: 0x060039CD RID: 14797 RVA: 0x000FD6B4 File Offset: 0x000FB8B4
	private bool IsCovered
	{
		get
		{
			return this.isCovered && !this.ForceUncovered;
		}
	}

	// Token: 0x17000691 RID: 1681
	// (get) Token: 0x060039CE RID: 14798 RVA: 0x000FD6C9 File Offset: 0x000FB8C9
	private float BlackoutFadeTime
	{
		get
		{
			if (Time.frameCount - 1 <= this.heroReadyFrame || !this.hc || !this.hc.isHeroInPosition)
			{
				return 0f;
			}
			return this.fadeTime;
		}
	}

	// Token: 0x060039CF RID: 14799 RVA: 0x000FD700 File Offset: 0x000FB900
	private void Reset()
	{
		this.persistent = base.GetComponent<PersistentBoolItem>();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060039D0 RID: 14800 RVA: 0x000FD71C File Offset: 0x000FB91C
	protected override void Awake()
	{
		base.Awake();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.hasBeenUncovered;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.hasBeenUncovered = value;
			};
		}
		if (this.blackoutMask)
		{
			SpriteRenderer component = base.GetComponent<SpriteRenderer>();
			if (component)
			{
				SpriteRenderer component2 = new GameObject("SpriteChild", new Type[]
				{
					typeof(SpriteRenderer)
				}).GetComponent<SpriteRenderer>();
				component2.transform.SetParentReset(base.transform);
				component2.sprite = component.sprite;
				NestedFadeGroupSpriteRenderer component3 = component.GetComponent<NestedFadeGroupSpriteRenderer>();
				if (component3)
				{
					Object.DestroyImmediate(component3);
				}
				Object.DestroyImmediate(component);
			}
			Material cutoutSpriteMaterial = Effects.CutoutSpriteMaterial;
			this.childSprites = base.GetComponentsInChildren<SpriteRenderer>(true);
			foreach (SpriteRenderer spriteRenderer in this.childSprites)
			{
				spriteRenderer.sharedMaterial = cutoutSpriteMaterial;
				spriteRenderer.gameObject.layer = 1;
				spriteRenderer.sortingLayerName = "Over";
			}
		}
	}

	// Token: 0x060039D1 RID: 14801 RVA: 0x000FD834 File Offset: 0x000FBA34
	protected override void Start()
	{
		base.Start();
		if (!Application.isPlaying)
		{
			return;
		}
		this.hc = HeroController.instance;
		this.hc.OnDeath += this.OnHeroDeath;
		this.hc.OnHazardDeath += this.OnHeroHazardDeath;
		this.hc.OnHazardRespawn += this.OnHeroHazardRespawn;
		this.hc.heroInPosition += this.InitialHeroInPosition;
		this.gm = GameManager.instance;
		this.gm.OnFinishedEnteringScene += this.OnHeroFinishedEnteringScene;
		if (this.hc.isHeroInPosition)
		{
			this.OnHeroInPosition(false);
		}
		else
		{
			this.hc.heroInPosition += this.OnHeroInPosition;
			this.subscribedHeroInPosition = true;
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "REMASKER FREEZE").ReceivedEvent += delegate()
		{
			this.isFrozen = true;
		};
	}

	// Token: 0x060039D2 RID: 14802 RVA: 0x000FD930 File Offset: 0x000FBB30
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.hc)
		{
			this.hc.OnDeath -= this.OnHeroDeath;
			this.hc.OnHazardDeath -= this.OnHeroHazardDeath;
			this.hc.OnHazardRespawn -= this.OnHeroHazardRespawn;
			this.hc.heroInPosition -= this.InitialHeroInPosition;
		}
		if (this.gm)
		{
			this.gm.OnFinishedEnteringScene -= this.OnHeroFinishedEnteringScene;
		}
		if (Remasker._wasLastExited == this)
		{
			Remasker._wasLastExited = null;
		}
		MaskerBlackout.RemoveInside(this, this.BlackoutFadeTime);
	}

	// Token: 0x060039D3 RID: 14803 RVA: 0x000FD9F8 File Offset: 0x000FBBF8
	private void OnHeroDeath()
	{
		this.isFrozen = true;
	}

	// Token: 0x060039D4 RID: 14804 RVA: 0x000FDA01 File Offset: 0x000FBC01
	private void OnHeroHazardDeath()
	{
		this.hazardDisabled = true;
		this.hazardEndIsInside = this.isInside;
	}

	// Token: 0x060039D5 RID: 14805 RVA: 0x000FDA16 File Offset: 0x000FBC16
	private void OnHeroHazardRespawn()
	{
		this.hazardDisabled = false;
		if (this.isInside)
		{
			if (!this.hazardEndIsInside)
			{
				this.Exited(false);
				return;
			}
		}
		else if (this.hazardEndIsInside)
		{
			this.Entered();
		}
	}

	// Token: 0x060039D6 RID: 14806 RVA: 0x000FDA45 File Offset: 0x000FBC45
	private void OnHeroFinishedEnteringScene()
	{
		if (!this.isInside && !this.isCovered)
		{
			this.isCovered = true;
		}
	}

	// Token: 0x060039D7 RID: 14807 RVA: 0x000FDA60 File Offset: 0x000FBC60
	private void InitialHeroInPosition(bool forcedirect)
	{
		if (this.hasSetInitialHeroPosition)
		{
			return;
		}
		this.hasSetInitialHeroPosition = true;
		if (!this.isInside)
		{
			this.isCovered = true;
		}
		this.heroReadyFrame = Time.frameCount;
		if (this.blackoutMask && this.isInside)
		{
			MaskerBlackout.SetMaskFade(1f);
			return;
		}
		if (this.exitQueued)
		{
			this.exitQueued = false;
			if (MaskerBlackout.RemoveInside(this, 0f))
			{
				Remasker._wasLastExited = this;
			}
		}
	}

	// Token: 0x060039D8 RID: 14808 RVA: 0x000FDAD4 File Offset: 0x000FBCD4
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.blackoutMask)
		{
			base.AlphaSelf = 1f;
			this.SetColor(Color.green);
		}
		else
		{
			base.AlphaSelf = (float)(this.inverse ? 0 : 1);
		}
		this.isCovered = true;
		if (this.hc && this.hc.isHeroInPosition)
		{
			this.OnHeroInPosition(false);
		}
	}

	// Token: 0x060039D9 RID: 14809 RVA: 0x000FDB4C File Offset: 0x000FBD4C
	protected override void OnDisable()
	{
		base.OnDisable();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.hc && this.subscribedHeroInPosition)
		{
			this.hc.heroInPosition -= this.OnHeroInPosition;
			this.subscribedHeroInPosition = false;
		}
		this.StopFadeWatchRoutine();
		if (this.isInside)
		{
			this.Exited(true);
		}
	}

	// Token: 0x060039DA RID: 14810 RVA: 0x000FDBB0 File Offset: 0x000FBDB0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!collision.CompareTag("Player"))
		{
			return;
		}
		if (this.hazardDisabled)
		{
			this.hazardEndIsInside = true;
			return;
		}
		if (this.hc && this.hc.cState.isTriggerEventsPaused)
		{
			return;
		}
		this.Entered();
	}

	// Token: 0x060039DB RID: 14811 RVA: 0x000FDC0C File Offset: 0x000FBE0C
	private void Entered()
	{
		this.isInside = true;
		if (this.isFrozen)
		{
			return;
		}
		if (this.blackoutMask)
		{
			if (MaskerBlackout.AddInside(this, this.BlackoutFadeTime))
			{
				this.SetColor(Color.blue);
			}
			else
			{
				Color color = this.lastSetColor;
				color.g = 0f;
				this.SetColor(color);
			}
			Remasker._wasLastExited = null;
		}
		this.exitQueued = false;
		this.isCovered = false;
	}

	// Token: 0x060039DC RID: 14812 RVA: 0x000FDC7C File Offset: 0x000FBE7C
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!collision.CompareTag("Player"))
		{
			return;
		}
		if (this.hazardDisabled)
		{
			this.hazardEndIsInside = false;
			return;
		}
		if (this.hc && this.hc.cState.isTriggerEventsPaused)
		{
			return;
		}
		this.Exited(false);
	}

	// Token: 0x060039DD RID: 14813 RVA: 0x000FDCD8 File Offset: 0x000FBED8
	public void Exited(bool wasDisabled)
	{
		this.isInside = false;
		if (this.isFrozen)
		{
			return;
		}
		if (this.hc.cState.hazardDeath)
		{
			return;
		}
		if (!wasDisabled && this.hc.transitionState == HeroTransitionState.EXITING_SCENE)
		{
			this.exitQueued = true;
			return;
		}
		if (this.blackoutMask)
		{
			float fadeOutTime = wasDisabled ? 0f : this.BlackoutFadeTime;
			if (MaskerBlackout.RemoveInside(this, fadeOutTime))
			{
				Remasker._wasLastExited = this;
			}
		}
		this.isCovered = true;
	}

	// Token: 0x060039DE RID: 14814 RVA: 0x000FDD50 File Offset: 0x000FBF50
	private void StopFadeWatchRoutine()
	{
		if (this.fadeWatchRoutine != null)
		{
			base.StopCoroutine(this.fadeWatchRoutine);
			this.fadeWatchRoutine = null;
		}
	}

	// Token: 0x060039DF RID: 14815 RVA: 0x000FDD70 File Offset: 0x000FBF70
	private void OnHeroInPosition(bool forceDirect)
	{
		this.InitialHeroInPosition(forceDirect);
		if (this.subscribedHeroInPosition)
		{
			this.hc.heroInPosition -= this.OnHeroInPosition;
			this.subscribedHeroInPosition = false;
		}
		this.StopFadeWatchRoutine();
		this.fadeWatchRoutine = base.StartCoroutine(this.FadeWatch());
	}

	// Token: 0x060039E0 RID: 14816 RVA: 0x000FDDC2 File Offset: 0x000FBFC2
	private IEnumerator FadeWatch()
	{
		this.<FadeWatch>g__SetInitialState|50_0();
		yield return null;
		this.<FadeWatch>g__SetInitialState|50_0();
		for (;;)
		{
			if (!this.IsCovered)
			{
				if (!this.hasBeenUncovered)
				{
					this.hasBeenUncovered = true;
					if (this.playSound && (!this.onlyPlaySoundInside || this.onlyPlaySoundInside.IsInside))
					{
						EventRegister.SendEvent("SECRET TONE", null);
					}
				}
				if (this.blackoutMask)
				{
					yield return base.StartCoroutine(this.FadeColor(Color.blue, this.fadeTime, false));
				}
				else
				{
					yield return base.StartCoroutine(this.FadeAlpha((float)(this.inverse ? 1 : 0), this.fadeTime, false));
				}
				while (!this.IsCovered)
				{
					yield return null;
				}
				if (this.blackoutMask)
				{
					if (Remasker._wasLastExited == this)
					{
						while (Remasker._wasLastExited == this)
						{
							if (!MaskerBlackout.IsAnyFading)
							{
								Remasker._wasLastExited = null;
								break;
							}
							yield return null;
						}
						this.SetColor(Color.green);
					}
					else
					{
						yield return base.StartCoroutine(this.FadeColor(Color.green, this.fadeTime, true));
					}
				}
				else
				{
					yield return base.StartCoroutine(this.FadeAlpha((float)(this.inverse ? 0 : 1), this.fadeTime, true));
				}
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x060039E1 RID: 14817 RVA: 0x000FDDD1 File Offset: 0x000FBFD1
	private IEnumerator FadeAlpha(float toAlpha, float time, bool targetCoveredState)
	{
		float fadingTime = base.FadeTo(toAlpha, time, null, false, null);
		float elapsed = 0f;
		while (elapsed < fadingTime && this.IsCovered == targetCoveredState)
		{
			yield return null;
			elapsed += Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x060039E2 RID: 14818 RVA: 0x000FDDF5 File Offset: 0x000FBFF5
	private IEnumerator FadeColor(Color toColor, float time, bool targetCoveredState)
	{
		Color fromColor = this.lastSetColor;
		float elapsed = 0f;
		while (elapsed < time && this.IsCovered == targetCoveredState)
		{
			float t = elapsed / time;
			this.SetColor(Color.Lerp(fromColor, toColor, t));
			yield return null;
			elapsed += Time.deltaTime;
		}
		if (this.IsCovered == targetCoveredState)
		{
			this.SetColor(toColor);
		}
		yield break;
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x000FDE1C File Offset: 0x000FC01C
	protected override void OnAlphaChanged(float alpha)
	{
		base.OnAlphaChanged(alpha);
		if (!this.linkedInverseGroup)
		{
			return;
		}
		this.linkedInverseGroup.AlphaSelf = ((MaskerBase.ApplyToInverseMasks && (MaskerBase.UseTestingAlphaInPlayMode || !Application.isPlaying)) ? alpha : (1f - alpha));
	}

	// Token: 0x060039E4 RID: 14820 RVA: 0x000FDE68 File Offset: 0x000FC068
	private void SetColor(Color color)
	{
		this.lastSetColor = color;
		foreach (SpriteRenderer spriteRenderer in this.childSprites)
		{
			color.a = spriteRenderer.color.a;
			spriteRenderer.color = color;
		}
	}

	// Token: 0x060039E5 RID: 14821 RVA: 0x000FDEAE File Offset: 0x000FC0AE
	public void SetPlaySound(bool setPlaySound)
	{
		this.playSound = setPlaySound;
	}

	// Token: 0x060039E6 RID: 14822 RVA: 0x000FDEB7 File Offset: 0x000FC0B7
	public void OnPrefabInstanceFix()
	{
		if (this.persistent)
		{
			ScenePrefabInstanceFix.CheckField<PersistentBoolItem>(ref this.persistent);
		}
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x000FDF08 File Offset: 0x000FC108
	[CompilerGenerated]
	private void <FadeWatch>g__SetInitialState|50_0()
	{
		if (this.blackoutMask)
		{
			this.SetColor((this.IsCovered && !this.isInside) ? Color.green : Color.blue);
			return;
		}
		if (this.IsCovered && !this.isInside)
		{
			base.AlphaSelf = (float)(this.inverse ? 0 : 1);
			return;
		}
		base.AlphaSelf = (float)(this.inverse ? 1 : 0);
	}

	// Token: 0x04003C75 RID: 15477
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003C76 RID: 15478
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003C77 RID: 15479
	[Space]
	[SerializeField]
	private float fadeTime = 0.5f;

	// Token: 0x04003C78 RID: 15480
	[SerializeField]
	private bool playSound;

	// Token: 0x04003C79 RID: 15481
	[SerializeField]
	private TrackTriggerObjects onlyPlaySoundInside;

	// Token: 0x04003C7A RID: 15482
	[SerializeField]
	[ModifiableProperty]
	[Conditional("blackoutMask", false, false, false)]
	private bool inverse;

	// Token: 0x04003C7B RID: 15483
	[SerializeField]
	private bool blackoutMask;

	// Token: 0x04003C7C RID: 15484
	[Space]
	[SerializeField]
	private NestedFadeGroupBase linkedInverseGroup;

	// Token: 0x04003C7D RID: 15485
	[SerializeField]
	private bool debugMe;

	// Token: 0x04003C7E RID: 15486
	private bool isCovered;

	// Token: 0x04003C7F RID: 15487
	private bool hasBeenUncovered;

	// Token: 0x04003C80 RID: 15488
	private bool isFrozen;

	// Token: 0x04003C81 RID: 15489
	private bool isInside;

	// Token: 0x04003C82 RID: 15490
	private bool hazardDisabled;

	// Token: 0x04003C83 RID: 15491
	private bool hazardEndIsInside;

	// Token: 0x04003C84 RID: 15492
	private int heroReadyFrame = -1;

	// Token: 0x04003C85 RID: 15493
	private Coroutine fadeWatchRoutine;

	// Token: 0x04003C86 RID: 15494
	private SpriteRenderer[] childSprites;

	// Token: 0x04003C87 RID: 15495
	private Color lastSetColor;

	// Token: 0x04003C88 RID: 15496
	private static Remasker _wasLastExited;

	// Token: 0x04003C89 RID: 15497
	private bool subscribedHeroInPosition;

	// Token: 0x04003C8A RID: 15498
	[NonSerialized]
	private bool hasSetInitialHeroPosition;

	// Token: 0x04003C8B RID: 15499
	private HeroController hc;

	// Token: 0x04003C8C RID: 15500
	private GameManager gm;

	// Token: 0x04003C8D RID: 15501
	private bool exitQueued;
}
