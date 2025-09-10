using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class CorpseGoopExplode : Corpse
{
	// Token: 0x06001872 RID: 6258 RVA: 0x00070529 File Offset: 0x0006E729
	protected override void LandEffects()
	{
		base.LandEffects();
		base.StartCoroutine(this.DoLandEffects());
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x0007053E File Offset: 0x0006E73E
	private IEnumerator DoLandEffects()
	{
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector3.zero;
		yield return new WaitForSeconds(1f);
		this.body.linearVelocity = Vector2.zero;
		this.gushSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		yield return base.StartCoroutine(this.Jitter(0.7f));
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if (gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		if (this.gasExplosion)
		{
			Object.Instantiate<GameObject>(this.gasExplosion, base.transform.position, Quaternion.identity);
		}
		this.meshRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x0007054D File Offset: 0x0006E74D
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

	// Token: 0x04001774 RID: 6004
	[Header("Goop Explode Variables")]
	public GameObject gasExplosion;

	// Token: 0x04001775 RID: 6005
	public AudioEvent gushSound;
}
