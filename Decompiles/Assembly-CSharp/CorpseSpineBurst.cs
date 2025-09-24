using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class CorpseSpineBurst : Corpse
{
	// Token: 0x060018A4 RID: 6308 RVA: 0x00071057 File Offset: 0x0006F257
	protected override void LandEffects()
	{
		base.LandEffects();
		base.StartCoroutine(this.DoLandEffects(true));
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x0007106D File Offset: 0x0006F26D
	private IEnumerator DoLandEffects(bool burst = true)
	{
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector3.zero;
		if (burst)
		{
			yield return new WaitForSeconds(1f);
			this.spriteAnimator.Play("Burst Antic");
			this.shakerExplode.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			yield return new WaitForSeconds(0.9f);
			this.spriteAnimator.Play("Burst");
			this.zombiePrep.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			this.zombieShoot.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			if (this.spineHit)
			{
				this.spineHit.SetActive(true);
			}
			if (this.lines)
			{
				this.lines.SetActive(true);
			}
			if (Vector2.Distance(HeroController.instance.transform.position, base.transform.position) <= 44f)
			{
				GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
				if (gameCameras)
				{
					gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		HealthManager component = base.GetComponent<HealthManager>();
		if (component)
		{
			component.IsInvincible = false;
		}
		yield break;
	}

	// Token: 0x0400179E RID: 6046
	[Header("Spine Burst Variables")]
	public AudioEvent shakerExplode;

	// Token: 0x0400179F RID: 6047
	public AudioEvent zombiePrep;

	// Token: 0x040017A0 RID: 6048
	public AudioEvent zombieShoot;

	// Token: 0x040017A1 RID: 6049
	[Space]
	public GameObject spineHit;

	// Token: 0x040017A2 RID: 6050
	public GameObject lines;
}
