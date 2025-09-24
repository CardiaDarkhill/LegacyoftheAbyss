using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class CorpseFungusExplode : Corpse
{
	// Token: 0x0600186E RID: 6254 RVA: 0x000704E7 File Offset: 0x0006E6E7
	protected override void LandEffects()
	{
		base.LandEffects();
		base.StartCoroutine(this.DoLandEffects());
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x000704FC File Offset: 0x0006E6FC
	private IEnumerator DoLandEffects()
	{
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector3.zero;
		yield return new WaitForSeconds(1f);
		if (this.anticSteam)
		{
			this.anticSteam.Play();
		}
		this.body.linearVelocity = Vector2.zero;
		this.gushSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		yield return base.StartCoroutine(this.Jitter(0.9f));
		this.explodeSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		if (this.anticSteam)
		{
			this.anticSteam.Stop();
		}
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if (gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		if (this.gasAttack)
		{
			this.gasAttack.Play();
		}
		if (this.gasHitBox)
		{
			this.gasHitBox.SetActive(true);
			Vector3 localScale = this.gasHitBox.transform.localScale;
			this.gasHitBox.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			iTween.ScaleTo(this.gasHitBox, iTween.Hash(new object[]
			{
				"scale",
				localScale,
				"time",
				0.4f,
				"easetype",
				iTween.EaseType.easeOutCirc,
				"islocal",
				true
			}));
			yield return new WaitForSeconds(0.4f);
		}
		this.meshRenderer.enabled = false;
		yield return new WaitForSeconds(0.4f);
		if (this.gasHitBox)
		{
			this.gasHitBox.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x0007050B File Offset: 0x0006E70B
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

	// Token: 0x0400176F RID: 5999
	[Header("Fungus Explode Variables")]
	public ParticleSystem anticSteam;

	// Token: 0x04001770 RID: 6000
	public ParticleSystem gasAttack;

	// Token: 0x04001771 RID: 6001
	public AudioEvent gushSound;

	// Token: 0x04001772 RID: 6002
	public AudioEvent explodeSound;

	// Token: 0x04001773 RID: 6003
	public GameObject gasHitBox;
}
