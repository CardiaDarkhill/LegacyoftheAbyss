using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200051C RID: 1308
public class MinesFlipPlatform : MonoBehaviour
{
	// Token: 0x06002F15 RID: 12053 RVA: 0x000CFD6E File Offset: 0x000CDF6E
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x000CFD7C File Offset: 0x000CDF7C
	private void Start()
	{
		this.idleRoutine = base.StartCoroutine(this.Idle());
		if (this.topSpikes)
		{
			this.triggerEnter = this.topSpikes.GetComponent<TriggerEnterEvent>();
		}
		if (this.triggerEnter)
		{
			this.triggerEnter.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
			{
				if (collider.tag == "Nail Attack")
				{
					this.hitCancel = true;
				}
			};
		}
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x000CFDE0 File Offset: 0x000CDFE0
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (this.idleRoutine != null)
			{
				base.StopCoroutine(this.idleRoutine);
			}
			if (this.flipRoutine == null)
			{
				this.flipRoutine = base.StartCoroutine(this.Flip());
			}
		}
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x000CFE32 File Offset: 0x000CE032
	private void PlaySound(AudioClip clip)
	{
		if (this.audioSource && clip)
		{
			this.audioSource.PlayOneShot(clip);
		}
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x000CFE55 File Offset: 0x000CE055
	private IEnumerator Idle()
	{
		for (;;)
		{
			this.spriteAnimator.Play("Idle Up");
			yield return new WaitForSeconds(Random.Range(2f, 5f));
			tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName("Glimmer Up");
			this.spriteAnimator.Play(clipByName);
			yield return new WaitForSeconds(clipByName.Duration);
		}
		yield break;
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x000CFE64 File Offset: 0x000CE064
	private IEnumerator Flip()
	{
		this.PlaySound(this.flipSound);
		this.spriteAnimator.Play("Shake Up");
		if (this.strikeEffect)
		{
			this.strikeEffect.SetActive(true);
		}
		base.StartCoroutine(this.Jitter(0.5f));
		yield return new WaitForSeconds(0.5f);
		if (this.crystalParticles)
		{
			this.crystalParticles.Play();
		}
		yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait("Flip Down 1", null));
		if (this.crystalParticles)
		{
			this.crystalParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait("Flip Down 2", null));
		if (this.topSpikes)
		{
			this.topSpikes.SetActive(true);
		}
		if (this.bottomSpikes)
		{
			this.bottomSpikes.SetActive(false);
		}
		this.spriteAnimator.Play("Idle Down");
		this.hitCancel = false;
		bool skipped = false;
		float elapsed = 0f;
		while (elapsed < 4f)
		{
			if (this.hitCancel)
			{
				skipped = true;
				if (this.nailStrikePrefab)
				{
					this.nailStrikePrefab.Spawn(base.transform.position);
				}
				if (this.crystalHitParticles)
				{
					this.crystalHitParticles.Play();
				}
				this.PlaySound(this.hitSound);
				GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
				if (gameCameras)
				{
					gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
					break;
				}
				break;
			}
			else
			{
				yield return null;
				elapsed += Time.deltaTime;
			}
		}
		this.PlaySound(this.flipBackSound);
		yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait(skipped ? "Flip Up 1N" : "Flip Up 1", null));
		yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait("Flip Up 2", null));
		if (this.topSpikes)
		{
			this.topSpikes.SetActive(false);
		}
		if (this.bottomSpikes)
		{
			this.bottomSpikes.SetActive(true);
		}
		this.flipRoutine = null;
		this.idleRoutine = base.StartCoroutine(this.Idle());
		yield break;
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x000CFE73 File Offset: 0x000CE073
	private IEnumerator Jitter(float duration)
	{
		Transform sprite = this.spriteAnimator.transform;
		Vector3 initialPos = sprite.position;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			sprite.position = initialPos + new Vector3(Random.Range(-0.1f, 0.1f), 0f, 0f);
			yield return null;
		}
		sprite.position = initialPos;
		yield break;
	}

	// Token: 0x040031C4 RID: 12740
	public tk2dSpriteAnimator spriteAnimator;

	// Token: 0x040031C5 RID: 12741
	[Space]
	public AudioClip flipSound;

	// Token: 0x040031C6 RID: 12742
	public AudioClip flipBackSound;

	// Token: 0x040031C7 RID: 12743
	public AudioClip hitSound;

	// Token: 0x040031C8 RID: 12744
	[Space]
	public GameObject strikeEffect;

	// Token: 0x040031C9 RID: 12745
	public GameObject nailStrikePrefab;

	// Token: 0x040031CA RID: 12746
	[Space]
	public ParticleSystem crystalParticles;

	// Token: 0x040031CB RID: 12747
	public ParticleSystem crystalHitParticles;

	// Token: 0x040031CC RID: 12748
	[Space]
	public GameObject topSpikes;

	// Token: 0x040031CD RID: 12749
	public GameObject bottomSpikes;

	// Token: 0x040031CE RID: 12750
	private Coroutine idleRoutine;

	// Token: 0x040031CF RID: 12751
	private Coroutine flipRoutine;

	// Token: 0x040031D0 RID: 12752
	private bool hitCancel;

	// Token: 0x040031D1 RID: 12753
	private TriggerEnterEvent triggerEnter;

	// Token: 0x040031D2 RID: 12754
	private AudioSource audioSource;
}
