using System;
using UnityEngine;

// Token: 0x0200039E RID: 926
public class BossStatueLever : MonoBehaviour, IBossStatueToggle
{
	// Token: 0x06001F3E RID: 7998 RVA: 0x0008EE07 File Offset: 0x0008D007
	private void Enable()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x0008EE18 File Offset: 0x0008D018
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.gameObject.activeInHierarchy || !this.canToggle)
		{
			return;
		}
		if (collision.tag == "Nail Attack")
		{
			this.bossStatue.SetDreamVersion(!this.bossStatue.UsingDreamVersion, true, true);
			this.canToggle = false;
			this.switchSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			GameManager.instance.FreezeMoment(1);
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			if (this.strikeNailPrefab && this.hitOrigin)
			{
				this.strikeNailPrefab.Spawn(this.hitOrigin.transform.position);
			}
			if (this.leverAnimator)
			{
				this.leverAnimator.Play("Hit");
			}
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x0008EF02 File Offset: 0x0008D102
	public void SetOwner(BossStatue statue)
	{
		this.bossStatue = statue;
		if (this.bossStatue.UsingDreamVersion)
		{
			this.bossStatue.SetDreamVersion(true, true, false);
		}
		this.bossStatue.OnStatueSwapFinished += delegate()
		{
			this.canToggle = true;
			if (this.leverAnimator)
			{
				this.leverAnimator.Play("Shine");
			}
		};
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x0008EF3D File Offset: 0x0008D13D
	public void SetState(bool value)
	{
		this.canToggle = value;
		if (!value)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001E2F RID: 7727
	public Transform hitOrigin;

	// Token: 0x04001E30 RID: 7728
	public AudioSource audioPlayerPrefab;

	// Token: 0x04001E31 RID: 7729
	public AudioEvent switchSound;

	// Token: 0x04001E32 RID: 7730
	public GameObject strikeNailPrefab;

	// Token: 0x04001E33 RID: 7731
	private bool canToggle = true;

	// Token: 0x04001E34 RID: 7732
	public Animator leverAnimator;

	// Token: 0x04001E35 RID: 7733
	private BossStatue bossStatue;
}
