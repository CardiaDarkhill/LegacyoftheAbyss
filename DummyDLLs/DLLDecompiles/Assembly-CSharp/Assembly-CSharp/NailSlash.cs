using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class NailSlash : NailAttackBase
{
	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0004BDF5 File Offset: 0x00049FF5
	// (set) Token: 0x06000FBF RID: 4031 RVA: 0x0004BDFD File Offset: 0x00049FFD
	public bool IsSlashOut { get; private set; }

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0004BE06 File Offset: 0x0004A006
	// (set) Token: 0x06000FC1 RID: 4033 RVA: 0x0004BE0E File Offset: 0x0004A00E
	public bool IsStartingSlash { get; private set; }

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0004BE18 File Offset: 0x0004A018
	protected override void Awake()
	{
		base.Awake();
		this.audio = base.GetComponent<AudioSource>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.poly = base.GetComponent<PolygonCollider2D>();
		this.hasPoly = (this.poly != null);
		this.mesh = base.GetComponent<MeshRenderer>();
		this.travel = base.GetComponent<NailSlashTravel>();
		this.originalPitch = this.audio.pitch;
		if (!this.travel)
		{
			this.hc.FlippedSprite += this.OnHeroFlipped;
		}
		this.poly.enabled = false;
		if (this.mesh)
		{
			this.mesh.enabled = false;
		}
		if (!this.dontAddRecoiler)
		{
			NailSlashRecoil.Add(base.gameObject, this.enemyDamager, this.drillPull);
		}
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0004BEF2 File Offset: 0x0004A0F2
	private void OnDestroy()
	{
		this.hc.FlippedSprite -= this.OnHeroFlipped;
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x0004BF0C File Offset: 0x0004A10C
	public void StartSlash()
	{
		if (this.setSlashComponent)
		{
			this.hc.SetSlashComponent(this);
		}
		this.OnSlashStarting();
		float num = this.originalPitch;
		if (this.hc.IsUsingQuickening)
		{
			num += 0.05f;
		}
		this.audio.pitch = num;
		this.audio.Play();
		this.animTriggerCounter = 0;
		this.poly.enabled = false;
		this.queuedDownspikeBounce = false;
		base.IsDamagerActive = true;
		this.IsStartingSlash = true;
		this.PlaySlash();
		if (this.drillPull)
		{
			this.hc.DrillDash(this.hc.transform.localScale.x < 0f);
		}
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0004BFC3 File Offset: 0x0004A1C3
	public void CancelAttack()
	{
		this.CancelAttack(true);
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x0004BFCC File Offset: 0x0004A1CC
	public void CancelAttack(bool forceHide)
	{
		this.IsStartingSlash = false;
		this.SetCollidersActive(false);
		if (this.mesh && (forceHide || (this.bounceConfig && this.bounceConfig.HideSlashOnBounceCancel)))
		{
			this.mesh.enabled = false;
		}
		base.IsDamagerActive = false;
		this.anim.AnimationEventTriggered = null;
		this.queuedDownspikeBounce = false;
		base.OnCancelAttack();
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x0004C040 File Offset: 0x0004A240
	private void PlaySlash()
	{
		if (this.mesh)
		{
			this.mesh.enabled = true;
		}
		this.anim.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered);
		this.anim.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
		float num = this.hc.IsUsingQuickening ? this.hc.Config.QuickAttackSpeedMult : 1f;
		tk2dSpriteAnimationClip clipByName = this.anim.GetClipByName(this.animName);
		this.anim.Play(clipByName, Mathf.Epsilon, clipByName.fps * num);
		base.OnPlaySlash();
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x0004C0EC File Offset: 0x0004A2EC
	private void OnAnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
	{
		if (!(clip.frames[frame].eventInfo == "Bounce"))
		{
			this.animTriggerCounter++;
			if (this.animTriggerCounter == 1)
			{
				this.IsStartingSlash = false;
				this.SetCollidersActive(true);
				if (base.ExtraDamager)
				{
					base.ExtraDamager.SetActive(true);
				}
				this.IsSlashOut = true;
			}
			if (this.animTriggerCounter == 2)
			{
				this.SetCollidersActive(false);
				if (base.ExtraDamager)
				{
					base.ExtraDamager.SetActive(false);
				}
				base.IsDamagerActive = false;
				this.enemyDamager.EndDamage();
				this.IsSlashOut = false;
			}
			return;
		}
		if (this.queuedDownspikeBounce)
		{
			this.queuedDownspikeBounce = false;
			this.DownBounce();
			return;
		}
		this.queuedDownspikeBounce = true;
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x0004C1B7 File Offset: 0x0004A3B7
	public void SetCollidersActive(bool value)
	{
		if (this.hasPoly)
		{
			this.poly.enabled = value;
		}
		if (this.clashTinkPoly)
		{
			this.clashTinkPoly.enabled = value;
		}
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0004C1E6 File Offset: 0x0004A3E6
	private void OnAnimationCompleted(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		this.CancelAttack();
		this.anim.AnimationCompleted = null;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0004C1FA File Offset: 0x0004A3FA
	private void OnHeroFlipped()
	{
		if (base.isActiveAndEnabled)
		{
			this.CancelAttack();
		}
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0004C20A File Offset: 0x0004A40A
	private void DownBounce()
	{
		this.hc.CancelAttack();
		this.DoDownspikeBounce();
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0004C21D File Offset: 0x0004A41D
	public void DoDownspikeBounce()
	{
		this.hc.DownspikeBounce(false, this.bounceConfig);
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0004C231 File Offset: 0x0004A431
	public override void QueueBounce()
	{
		base.QueueBounce();
		if (this.queuedDownspikeBounce)
		{
			this.queuedDownspikeBounce = false;
			this.DownBounce();
			return;
		}
		this.hc.DownspikeBounce(false, null);
		this.queuedDownspikeBounce = true;
	}

	// Token: 0x04000F55 RID: 3925
	public string animName;

	// Token: 0x04000F56 RID: 3926
	[Space]
	[SerializeField]
	[AssetPickerDropdown]
	private HeroSlashBounceConfig bounceConfig;

	// Token: 0x04000F57 RID: 3927
	private tk2dSpriteAnimator anim;

	// Token: 0x04000F58 RID: 3928
	private MeshRenderer mesh;

	// Token: 0x04000F59 RID: 3929
	private NailSlashTravel travel;

	// Token: 0x04000F5A RID: 3930
	private bool queuedDownspikeBounce;

	// Token: 0x04000F5B RID: 3931
	private int animTriggerCounter;

	// Token: 0x04000F5C RID: 3932
	private PolygonCollider2D poly;

	// Token: 0x04000F5D RID: 3933
	private AudioSource audio;

	// Token: 0x04000F60 RID: 3936
	[SerializeField]
	[ModifiableProperty]
	[Conditional("dontAddRecoiler", false, false, false)]
	private bool drillPull;

	// Token: 0x04000F61 RID: 3937
	[SerializeField]
	private bool dontAddRecoiler;

	// Token: 0x04000F62 RID: 3938
	[Space]
	[SerializeField]
	private bool setSlashComponent;

	// Token: 0x04000F63 RID: 3939
	private bool hasPoly;

	// Token: 0x04000F64 RID: 3940
	private float originalPitch;
}
