using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class CorpseDeathStun : Corpse
{
	// Token: 0x06001864 RID: 6244 RVA: 0x0007045F File Offset: 0x0006E65F
	protected override void Start()
	{
		base.StartCoroutine(this.DeathStun());
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x0007046E File Offset: 0x0006E66E
	private IEnumerator DeathStun()
	{
		SpriteFlash spriteFlash = base.GetComponent<SpriteFlash>();
		if (spriteFlash)
		{
			spriteFlash.flashInfectedLoop();
		}
		Vector2 velocity = this.body.linearVelocity;
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector2.zero;
		yield return base.StartCoroutine(this.Jitter(0.75f));
		if (spriteFlash)
		{
			spriteFlash.CancelFlash();
		}
		Object.Instantiate<GameObject>(this.deathWaveInfectedPrefab, base.transform.position, Quaternion.identity).transform.localScale = new Vector3(2f, 2f, 2f);
		this.body.isKinematic = false;
		this.body.linearVelocity = velocity;
		base.Start();
		yield break;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x0007047D File Offset: 0x0006E67D
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

	// Token: 0x0400176C RID: 5996
	[SerializeField]
	protected GameObject deathWaveInfectedPrefab;
}
