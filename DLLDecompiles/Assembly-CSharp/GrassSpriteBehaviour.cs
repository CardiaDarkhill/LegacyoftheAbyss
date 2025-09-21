using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class GrassSpriteBehaviour : MonoBehaviour
{
	// Token: 0x06001507 RID: 5383 RVA: 0x0005F49B File Offset: 0x0005D69B
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x0005F4B5 File Offset: 0x0005D6B5
	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1.8f)
		{
			this.interaction = false;
		}
		this.Init();
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x0005F4E6 File Offset: 0x0005D6E6
	private void OnBecameVisible()
	{
		this.visible = true;
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x0005F4EF File Offset: 0x0005D6EF
	private void OnBecameInvisible()
	{
		this.visible = false;
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x0005F4F8 File Offset: 0x0005D6F8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.isCut && this.interaction && this.visible)
		{
			if (GrassCut.ShouldCut(collision))
			{
				this.animator.Play(this.cutAnimation);
				this.isCut = true;
				if (this.isWindy && this.deathParticlesWindy)
				{
					this.deathParticlesWindy.SetActive(true);
				}
				else if (this.deathParticles)
				{
					this.deathParticles.SetActive(true);
				}
				if (this.source && this.cutSounds.Length != 0)
				{
					this.source.PlayOneShot(this.cutSounds[Random.Range(0, this.cutSounds.Length)]);
				}
				if (this.cutEffectPrefab)
				{
					int num = (int)Mathf.Sign(collision.transform.position.x - base.transform.position.x);
					Vector3 position = (collision.transform.position + base.transform.position) / 2f;
					GameObject gameObject = this.cutEffectPrefab.Spawn(position);
					Vector3 localScale = gameObject.transform.localScale;
					localScale.x = Mathf.Abs(localScale.x) * (float)(-(float)num);
					gameObject.transform.localScale = localScale;
					return;
				}
			}
			else
			{
				if (!this.noPushAnimation)
				{
					this.animator.Play(this.isWindy ? this.pushWindyAnimation : this.pushAnimation);
				}
				if (this.source && this.pushSounds.Length != 0)
				{
					this.source.PlayOneShot(this.pushSounds[Random.Range(0, this.pushSounds.Length)]);
				}
			}
		}
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x0005F6B2 File Offset: 0x0005D8B2
	private void Init()
	{
		this.animator.Play(this.isWindy ? this.idleWindyAnimation : this.idleAnimation);
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x0005F6D5 File Offset: 0x0005D8D5
	public void SetWindy()
	{
		if (this.isCut)
		{
			return;
		}
		this.isWindy = true;
		this.noPushAnimation = true;
		this.Init();
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x0005F6F4 File Offset: 0x0005D8F4
	public void SetNotWindy()
	{
		if (this.isCut)
		{
			return;
		}
		this.isWindy = false;
		this.noPushAnimation = false;
		this.Init();
	}

	// Token: 0x0400138D RID: 5005
	[Header("Variables")]
	public bool isWindy;

	// Token: 0x0400138E RID: 5006
	public bool noPushAnimation;

	// Token: 0x0400138F RID: 5007
	[Space]
	public GameObject deathParticles;

	// Token: 0x04001390 RID: 5008
	public GameObject deathParticlesWindy;

	// Token: 0x04001391 RID: 5009
	public GameObject cutEffectPrefab;

	// Token: 0x04001392 RID: 5010
	[Space]
	public AudioClip[] pushSounds;

	// Token: 0x04001393 RID: 5011
	public AudioClip[] cutSounds;

	// Token: 0x04001394 RID: 5012
	[Header("Animation State Names")]
	public string idleAnimation = "Idle";

	// Token: 0x04001395 RID: 5013
	public string pushAnimation = "Push";

	// Token: 0x04001396 RID: 5014
	public string cutAnimation = "Dead";

	// Token: 0x04001397 RID: 5015
	[Space]
	public string idleWindyAnimation = "WindyIdle";

	// Token: 0x04001398 RID: 5016
	public string pushWindyAnimation = "WindyPush";

	// Token: 0x04001399 RID: 5017
	private bool isCut;

	// Token: 0x0400139A RID: 5018
	private bool interaction = true;

	// Token: 0x0400139B RID: 5019
	private bool visible;

	// Token: 0x0400139C RID: 5020
	private Animator animator;

	// Token: 0x0400139D RID: 5021
	private AudioSource source;
}
