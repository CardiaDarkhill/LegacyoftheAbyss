using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002AD RID: 685
public class CorpseDeathStunChunker : CorpseChunker
{
	// Token: 0x06001869 RID: 6249 RVA: 0x000704A3 File Offset: 0x0006E6A3
	protected override void Start()
	{
		base.StartCoroutine(this.DeathStun());
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x000704B2 File Offset: 0x0006E6B2
	private IEnumerator DeathStun()
	{
		if (this.stunSteam)
		{
			this.stunSteam.Play();
		}
		SpriteFlash spriteFlash = base.GetComponent<SpriteFlash>();
		if (spriteFlash)
		{
			spriteFlash.flashInfectedLoop();
		}
		Vector2 velocity = Vector2.zero;
		if (this.body)
		{
			velocity = this.body.linearVelocity;
			this.body.isKinematic = true;
			this.body.linearVelocity = Vector2.zero;
		}
		yield return base.StartCoroutine(this.Jitter(0.75f));
		if (spriteFlash)
		{
			spriteFlash.CancelFlash();
		}
		Object.Instantiate<GameObject>(this.deathWaveInfectedPrefab, base.transform.position, Quaternion.identity).transform.localScale = new Vector3(2f, 2f, 2f);
		if (this.body)
		{
			this.body.isKinematic = false;
			this.body.linearVelocity = velocity;
		}
		if (this.stunSteam)
		{
			this.stunSteam.Stop();
		}
		base.Start();
		yield break;
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x000704C1 File Offset: 0x0006E6C1
	private IEnumerator Jitter(float duration)
	{
		Transform sprite = this.spriteAnimator.transform;
		Vector3 initialPos = sprite.position;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			sprite.position = initialPos + new Vector3(Random.Range(-0.15f, 0.15f), 0f, 0f);
			yield return null;
		}
		sprite.position = initialPos;
		yield break;
	}

	// Token: 0x0400176D RID: 5997
	[Header("Death Stun Variables")]
	[SerializeField]
	protected GameObject deathWaveInfectedPrefab;

	// Token: 0x0400176E RID: 5998
	public ParticleSystem stunSteam;
}
