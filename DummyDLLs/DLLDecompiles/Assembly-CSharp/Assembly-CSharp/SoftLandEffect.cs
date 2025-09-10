using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200027D RID: 637
public class SoftLandEffect : MonoBehaviour
{
	// Token: 0x06001690 RID: 5776 RVA: 0x000656C0 File Offset: 0x000638C0
	private void OnEnable()
	{
		PlayerData instance = PlayerData.instance;
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.GetComponent<AudioSource>();
		}
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
		this.recycleTimer = 2f;
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance == null || !silentInstance.isHeroInPosition)
		{
			return;
		}
		switch (instance.environmentType)
		{
		case EnvironmentTypes.Grass:
			this.grassEffects.SetActive(true);
			this.audioSource.PlayOneShot(this.grassLandClip);
			return;
		case EnvironmentTypes.Bone:
			this.boneEffects.SetActive(true);
			this.audioSource.PlayOneShot(this.boneLandClip);
			return;
		case EnvironmentTypes.ShallowWater:
			this.audioSource.PlayOneShot(this.wetLandClip);
			return;
		case EnvironmentTypes.Metal:
			this.audioSource.PlayOneShot(this.metalLandClip);
			return;
		case EnvironmentTypes.NoEffect:
			return;
		case EnvironmentTypes.Moss:
		case EnvironmentTypes.RunningWater:
			this.PlaySplash();
			return;
		case EnvironmentTypes.Sand:
			this.dustEffects.SetActive(true);
			this.audioSource.PlayOneShot(this.sandLandClip);
			return;
		case EnvironmentTypes.Bell:
			this.dustEffects.SetActive(true);
			this.audioSource.PlayOneShot(this.bellLandClip);
			return;
		case EnvironmentTypes.WetMetal:
			this.audioSource.PlayOneShot(this.metalLandClip);
			this.PlaySplash();
			return;
		case EnvironmentTypes.ThinMetal:
			this.audioSource.PlayOneShot(this.metalLandClip);
			return;
		case EnvironmentTypes.Wood:
			this.audioSource.PlayOneShot(this.woodLandClip);
			return;
		case EnvironmentTypes.WetWood:
			this.audioSource.PlayOneShot(this.woodLandClip);
			this.PlaySplash();
			return;
		case EnvironmentTypes.PeakPuff:
			this.peakPuffEffects.SetActive(true);
			this.audioSource.PlayOneShot(this.peakPuffClip);
			return;
		case EnvironmentTypes.FlowerField:
			this.audioSource.PlayOneShot(this.grassLandClip);
			return;
		}
		this.dustEffects.SetActive(true);
		this.audioSource.PlayOneShot(this.softLandClip);
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x00065900 File Offset: 0x00063B00
	private void Update()
	{
		if (this.recycleTimer <= 0f)
		{
			base.gameObject.Recycle();
			return;
		}
		this.recycleTimer -= Time.deltaTime;
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x00065930 File Offset: 0x00063B30
	private void PlaySplash()
	{
		this.audioSource.PlayOneShot(this.wetLandClip);
		Color color;
		this.splash.color = (AreaEffectTint.IsActive(base.transform.position, out color) ? color : Color.white);
		this.splash.gameObject.SetActive(true);
		if (Random.Range(1, 100) > 50)
		{
			Transform transform = this.splash.transform;
			Vector3 localScale = transform.localScale;
			transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
		}
	}

	// Token: 0x04001502 RID: 5378
	[SerializeField]
	private GameObject dustEffects;

	// Token: 0x04001503 RID: 5379
	[SerializeField]
	private GameObject grassEffects;

	// Token: 0x04001504 RID: 5380
	[SerializeField]
	private GameObject boneEffects;

	// Token: 0x04001505 RID: 5381
	[SerializeField]
	private GameObject peakPuffEffects;

	// Token: 0x04001506 RID: 5382
	[SerializeField]
	private SpriteRenderer splash;

	// Token: 0x04001507 RID: 5383
	[SerializeField]
	private AudioClip softLandClip;

	// Token: 0x04001508 RID: 5384
	[SerializeField]
	private AudioClip wetLandClip;

	// Token: 0x04001509 RID: 5385
	[SerializeField]
	private AudioClip boneLandClip;

	// Token: 0x0400150A RID: 5386
	[SerializeField]
	private AudioClip woodLandClip;

	// Token: 0x0400150B RID: 5387
	[SerializeField]
	private AudioClip metalLandClip;

	// Token: 0x0400150C RID: 5388
	[SerializeField]
	private AudioClip bellLandClip;

	// Token: 0x0400150D RID: 5389
	[SerializeField]
	private AudioClip grassLandClip;

	// Token: 0x0400150E RID: 5390
	[SerializeField]
	private AudioClip sandLandClip;

	// Token: 0x0400150F RID: 5391
	[SerializeField]
	private AudioClip peakPuffClip;

	// Token: 0x04001510 RID: 5392
	private GameObject heroObject;

	// Token: 0x04001511 RID: 5393
	private AudioSource audioSource;

	// Token: 0x04001512 RID: 5394
	private Rigidbody2D heroRigidBody;

	// Token: 0x04001513 RID: 5395
	private tk2dSpriteAnimator jumpPuffAnimator;

	// Token: 0x04001514 RID: 5396
	private float recycleTimer;
}
