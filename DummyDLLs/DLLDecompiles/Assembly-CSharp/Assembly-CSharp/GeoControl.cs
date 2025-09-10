using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003DD RID: 989
public class GeoControl : CurrencyObject<GeoControl>, IBreakOnContact
{
	// Token: 0x060021D5 RID: 8661 RVA: 0x0009C0C8 File Offset: 0x0009A2C8
	private void CacheAnimations()
	{
		if (!this.hasAnim)
		{
			return;
		}
		this.landClip = (this.anim.GetClipByName(this.landAnim) ?? this.anim.GetClipByName(this.idleAnim));
		this.airClip = this.anim.GetClipByName(this.airAnim);
		this.gleamClip = this.anim.GetClipByName(this.gleamAnim);
		this.hasGleamAnim = (this.gleamClip != null);
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x0009C148 File Offset: 0x0009A348
	private void CacheAnimator()
	{
		if (this.hasAnimator)
		{
			return;
		}
		this.animator = base.GetComponent<Animator>();
		this.hasAnimator = (this.animator != null);
		if (this.hasAnimator)
		{
			this.airHash = Animator.StringToHash(this.airAnim);
			this.gleamHash = Animator.StringToHash(this.gleamAnim);
			this.idleHash = Animator.StringToHash(this.idleAnim);
			if (!string.IsNullOrEmpty(this.landAnim))
			{
				this.landHash = Animator.StringToHash(this.landAnim);
				return;
			}
			this.landHash = this.idleHash;
		}
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x060021D7 RID: 8663 RVA: 0x0009C1E2 File Offset: 0x0009A3E2
	protected override CurrencyType? CurrencyType
	{
		get
		{
			return new CurrencyType?(global::CurrencyType.Money);
		}
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x0009C1EC File Offset: 0x0009A3EC
	protected override void Awake()
	{
		base.Awake();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.CacheAnimator();
		this.hasAnim = this.anim;
		if (this.hasAnim)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.anim;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
			this.CacheAnimations();
		}
		this.hasValueReference = this.valueReference;
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x0009C268 File Offset: 0x0009A468
	protected override void Start()
	{
		base.Start();
		this.started = true;
		ComponentSingleton<GeoControlCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x0009C28D File Offset: 0x0009A48D
	private void OnValidate()
	{
		this.hasAnimator = false;
		this.CacheAnimator();
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x0009C29C File Offset: 0x0009A49C
	protected override void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<GeoControlCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		}
		base.OnEnable();
		this.PlayAir();
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x0009C2C8 File Offset: 0x0009A4C8
	protected override void OnDisable()
	{
		ComponentSingleton<GeoControlCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
		base.OnDisable();
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x0009C2E8 File Offset: 0x0009A4E8
	public override void CollectPopup()
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(this.firstGetPDBool))
		{
			if (!instance.HasSeenGeoMid && "HasSeenGeoMid" == this.firstGetPDBool)
			{
				instance.HasSeenGeoMid = true;
			}
			else if (!instance.HasSeenGeoBig && "HasSeenGeoBig" == this.firstGetPDBool)
			{
				instance.HasSeenGeoBig = true;
			}
		}
		if (instance.HasSeenGeo)
		{
			return;
		}
		instance.HasSeenGeo = true;
		if (!this.popupName.IsEmpty)
		{
			CollectableUIMsg.Spawn(new UIMsgDisplay
			{
				Name = this.popupName,
				Icon = this.popupSprite,
				IconScale = 1f
			}, null, false);
		}
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x0009C3AC File Offset: 0x0009A5AC
	public void OnUpdate()
	{
		if (!this.hasGleamAnim && this.gleamHash == 0)
		{
			return;
		}
		if (this.gleamDelay <= 0f)
		{
			return;
		}
		this.gleamDelay -= Time.deltaTime;
		if (this.gleamDelay > 0f)
		{
			return;
		}
		this.PlayGleam();
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x0009C3FE File Offset: 0x0009A5FE
	protected override void Land()
	{
		base.Land();
		this.PlayLand();
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x0009C40C File Offset: 0x0009A60C
	protected override void LeftGround()
	{
		base.LeftGround();
		this.PlayAir();
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x0009C41A File Offset: 0x0009A61A
	protected override bool Collected()
	{
		if (!this.hasValueReference)
		{
			return false;
		}
		CurrencyManager.AddGeo(this.valueReference.Value);
		return true;
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x0009C438 File Offset: 0x0009A638
	private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == this.landAnim || clip.name == this.gleamAnim)
		{
			animator.Play(this.idleAnim);
			this.gleamDelay = this.gleamDelayRange.GetRandomValue();
		}
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x0009C488 File Offset: 0x0009A688
	public void SpawnThiefCharmEffect()
	{
		if (!this.thiefCharmEffectPrefab)
		{
			return;
		}
		this.thiefCharmEffectPrefab.Spawn(base.transform, new Vector3(0f, 0f, -0.001f));
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x0009C4C0 File Offset: 0x0009A6C0
	private void PlayGleam()
	{
		if (this.hasAnimator)
		{
			this.animator.SetTrigger(this.gleamHash);
			this.gleamDelay = this.gleamDelayRange.GetRandomValue();
		}
		if (this.hasAnim)
		{
			this.anim.Play(this.gleamClip);
		}
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x0009C510 File Offset: 0x0009A710
	private void PlayLand()
	{
		if (this.hasAnimator)
		{
			if (this.airHash != 0)
			{
				this.animator.SetBool(this.airHash, false);
			}
			else
			{
				this.animator.Play(this.landHash);
			}
			this.CancelGleam();
			this.gleamDelay = this.gleamDelayRange.GetRandomValue();
		}
		if (!this.hasAnim)
		{
			return;
		}
		if (this.currentClip != this.landClip)
		{
			this.currentClip = this.landClip;
			this.anim.Play(this.landClip);
		}
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x0009C5A0 File Offset: 0x0009A7A0
	private void PlayAir()
	{
		if (this.hasAnimator)
		{
			this.animator.SetBool(this.airHash, true);
			this.CancelGleam();
		}
		if (this.hasAnim && this.currentClip != this.airClip)
		{
			this.currentClip = this.airClip;
			this.anim.Play(this.airClip);
		}
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x0009C600 File Offset: 0x0009A800
	private void CancelGleam()
	{
		if (this.hasAnimator)
		{
			this.animator.ResetTrigger(this.gleamHash);
			this.gleamDelay = 0f;
		}
	}

	// Token: 0x04002093 RID: 8339
	[SerializeField]
	private string airAnim;

	// Token: 0x04002094 RID: 8340
	[SerializeField]
	private string landAnim;

	// Token: 0x04002095 RID: 8341
	[SerializeField]
	private string idleAnim;

	// Token: 0x04002096 RID: 8342
	[SerializeField]
	private string gleamAnim;

	// Token: 0x04002097 RID: 8343
	[SerializeField]
	private MinMaxFloat gleamDelayRange;

	// Token: 0x04002098 RID: 8344
	[SerializeField]
	private GameObject thiefCharmEffectPrefab;

	// Token: 0x04002099 RID: 8345
	[Space]
	[SerializeField]
	private CostReference valueReference;

	// Token: 0x0400209A RID: 8346
	private tk2dSpriteAnimator anim;

	// Token: 0x0400209B RID: 8347
	private float gleamDelay;

	// Token: 0x0400209C RID: 8348
	private tk2dSpriteAnimationClip gleamClip;

	// Token: 0x0400209D RID: 8349
	private tk2dSpriteAnimationClip airClip;

	// Token: 0x0400209E RID: 8350
	private tk2dSpriteAnimationClip landClip;

	// Token: 0x0400209F RID: 8351
	private tk2dSpriteAnimationClip currentClip;

	// Token: 0x040020A0 RID: 8352
	private bool hasAnim;

	// Token: 0x040020A1 RID: 8353
	private bool hasGleamAnim;

	// Token: 0x040020A2 RID: 8354
	private bool hasValueReference;

	// Token: 0x040020A3 RID: 8355
	private bool hasAnimator;

	// Token: 0x040020A4 RID: 8356
	private Animator animator;

	// Token: 0x040020A5 RID: 8357
	private int airHash;

	// Token: 0x040020A6 RID: 8358
	private int gleamHash;

	// Token: 0x040020A7 RID: 8359
	private int idleHash;

	// Token: 0x040020A8 RID: 8360
	private int landHash;

	// Token: 0x040020A9 RID: 8361
	private bool started;
}
