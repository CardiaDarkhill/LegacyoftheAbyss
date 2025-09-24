using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class DashEffect : MonoBehaviour
{
	// Token: 0x0600147E RID: 5246 RVA: 0x0005C681 File Offset: 0x0005A881
	private void Awake()
	{
		this.gm = GameManager.instance;
		this.audioSource = base.gameObject.GetComponent<AudioSource>();
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0005C6A0 File Offset: 0x0005A8A0
	private void OnEnable()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0005C6FC File Offset: 0x0005A8FC
	public void Play(GameObject owner)
	{
		EnviroRegionListener component = base.GetComponent<EnviroRegionListener>();
		EnvironmentTypes environmentTypes = component ? component.CurrentEnvironmentType : this.gm.sm.environmentType;
		this.recycleTimer = 2f;
		switch (environmentTypes)
		{
		case EnvironmentTypes.Grass:
			this.PlayDashPuff();
			this.PlayGrass();
			return;
		case EnvironmentTypes.Bone:
			this.PlayDashPuff();
			this.PlayBone();
			return;
		case EnvironmentTypes.ShallowWater:
			this.PlaySpaEffects();
			return;
		case EnvironmentTypes.Moss:
			return;
		}
		this.PlayDashPuff();
		this.PlayDust();
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x0005C78B File Offset: 0x0005A98B
	private void PlayDashPuff()
	{
		this.heroDashPuff.SetActive(true);
		this.heroDashPuff_anim.PlayFromFrame(0);
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x0005C7A5 File Offset: 0x0005A9A5
	private void PlayDust()
	{
		this.dashDust.SetActive(true);
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x0005C7B3 File Offset: 0x0005A9B3
	private void PlayGrass()
	{
		this.dashGrass.SetActive(true);
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x0005C7C1 File Offset: 0x0005A9C1
	private void PlayBone()
	{
		this.dashBone.SetActive(true);
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0005C7CF File Offset: 0x0005A9CF
	private void PlaySpaEffects()
	{
		this.waterCut.SetActive(true);
		this.audioSource.PlayOneShot(this.splashClip);
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x0005C7EE File Offset: 0x0005A9EE
	private void Update()
	{
		if (this.recycleTimer > 0f)
		{
			this.recycleTimer -= Time.deltaTime;
			if (this.recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
	}

	// Token: 0x040012CF RID: 4815
	public GameObject heroDashPuff;

	// Token: 0x040012D0 RID: 4816
	public GameObject dashDust;

	// Token: 0x040012D1 RID: 4817
	public GameObject dashBone;

	// Token: 0x040012D2 RID: 4818
	public GameObject dashGrass;

	// Token: 0x040012D3 RID: 4819
	public GameObject waterCut;

	// Token: 0x040012D4 RID: 4820
	public tk2dSpriteAnimator heroDashPuff_anim;

	// Token: 0x040012D5 RID: 4821
	public AudioClip splashClip;

	// Token: 0x040012D6 RID: 4822
	private GameManager gm;

	// Token: 0x040012D7 RID: 4823
	private GameObject heroObject;

	// Token: 0x040012D8 RID: 4824
	private AudioSource audioSource;

	// Token: 0x040012D9 RID: 4825
	private Rigidbody2D heroRigidBody;

	// Token: 0x040012DA RID: 4826
	private tk2dSpriteAnimator jumpPuffAnimator;

	// Token: 0x040012DB RID: 4827
	private float recycleTimer;
}
