using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004D6 RID: 1238
public class DropPlatform : MonoBehaviour
{
	// Token: 0x06002C82 RID: 11394 RVA: 0x000C2CB8 File Offset: 0x000C0EB8
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x000C2CC6 File Offset: 0x000C0EC6
	private void Start()
	{
		this.Idle();
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x000C2CD0 File Offset: 0x000C0ED0
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.flipRoutine == null && collision.gameObject.layer == 9 && collision.collider.bounds.min.y > this.collider.bounds.max.y)
		{
			this.flipRoutine = base.StartCoroutine(this.Flip());
		}
	}

	// Token: 0x06002C85 RID: 11397 RVA: 0x000C2D38 File Offset: 0x000C0F38
	private void PlaySound(AudioClip clip)
	{
		if (this.audioSource && clip)
		{
			this.audioSource.PlayOneShot(clip);
		}
	}

	// Token: 0x06002C86 RID: 11398 RVA: 0x000C2D5B File Offset: 0x000C0F5B
	private void Idle()
	{
		base.transform.SetPositionZ(0.003f);
		this.spriteAnimator.Play(this.idleAnim);
		if (this.collider)
		{
			this.collider.enabled = true;
		}
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x000C2D97 File Offset: 0x000C0F97
	private IEnumerator Flip()
	{
		this.PlaySound(this.landSound);
		if (this.strikeEffect)
		{
			this.strikeEffect.SetActive(true);
		}
		yield return new WaitForSeconds(0.7f);
		if (this.collider)
		{
			this.collider.enabled = false;
		}
		this.PlaySound(this.dropSound);
		yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait(this.dropAnim, null));
		base.transform.SetPositionZ(0.007f);
		yield return new WaitForSeconds(1.5f);
		this.PlaySound(this.flipUpSound);
		yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait(this.raiseAnim, null));
		this.flipRoutine = null;
		this.Idle();
		yield break;
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x000C2DA6 File Offset: 0x000C0FA6
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

	// Token: 0x04002E1D RID: 11805
	public tk2dSpriteAnimator spriteAnimator;

	// Token: 0x04002E1E RID: 11806
	[Space]
	public string idleAnim = "Idle Small";

	// Token: 0x04002E1F RID: 11807
	public string dropAnim = "Drop Small";

	// Token: 0x04002E20 RID: 11808
	public string raiseAnim = "Raise Small";

	// Token: 0x04002E21 RID: 11809
	[Space]
	public AudioClip landSound;

	// Token: 0x04002E22 RID: 11810
	public AudioClip dropSound;

	// Token: 0x04002E23 RID: 11811
	public AudioClip flipUpSound;

	// Token: 0x04002E24 RID: 11812
	[Space]
	public GameObject strikeEffect;

	// Token: 0x04002E25 RID: 11813
	[Space]
	public Collider2D collider;

	// Token: 0x04002E26 RID: 11814
	private Coroutine flipRoutine;

	// Token: 0x04002E27 RID: 11815
	private AudioSource audioSource;
}
