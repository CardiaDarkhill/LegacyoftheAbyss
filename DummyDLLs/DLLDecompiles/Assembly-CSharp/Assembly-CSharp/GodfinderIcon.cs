using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003AE RID: 942
public class GodfinderIcon : MonoBehaviour
{
	// Token: 0x06001FAB RID: 8107 RVA: 0x00090CF3 File Offset: 0x0008EEF3
	private void Awake()
	{
		GodfinderIcon.instance = this;
		this.renderer = base.GetComponent<MeshRenderer>();
		this.spriteAnimator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x00090D13 File Offset: 0x0008EF13
	private void Start()
	{
		this.renderer.enabled = false;
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x00090D21 File Offset: 0x0008EF21
	private void Update()
	{
		if (this.isVisible && !this.spriteAnimator.Playing)
		{
			this.Hide();
		}
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x00090D40 File Offset: 0x0008EF40
	public static void ShowIcon(float delay, BossScene bossScene)
	{
		if (GameManager.instance.playerData.bossRushMode)
		{
			return;
		}
		if (GameManager.instance.playerData.hasGodfinder)
		{
			if (bossScene != null && bossScene.IsUnlocked(BossSceneCheckSource.Godfinder))
			{
				return;
			}
			GameManager.instance.playerData.unlockedNewBossStatue = true;
			if (GodfinderIcon.instance)
			{
				if (GameManager.instance.GetCurrentMapZone() != "GODS_GLORY")
				{
					GodfinderIcon.instance.StartCoroutine(GodfinderIcon.instance.Show(delay));
					return;
				}
				GameManager.instance.playerData.queuedGodfinderIcon = true;
			}
		}
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x00090DDC File Offset: 0x0008EFDC
	public static void ShowIconQueued(float delay)
	{
		if (GameManager.instance.playerData.bossRushMode)
		{
			return;
		}
		if (GodfinderIcon.instance && GameManager.instance.playerData.queuedGodfinderIcon)
		{
			GodfinderIcon.instance.StartCoroutine(GodfinderIcon.instance.Show(delay));
			GameManager.instance.playerData.queuedGodfinderIcon = false;
		}
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x00090E3E File Offset: 0x0008F03E
	private IEnumerator Show(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.renderer.enabled = true;
		this.spriteAnimator.PlayFromFrame(0);
		this.showSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		this.isVisible = true;
		yield break;
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x00090E54 File Offset: 0x0008F054
	private void Hide()
	{
		this.renderer.enabled = false;
		this.isVisible = false;
	}

	// Token: 0x04001EB8 RID: 7864
	private static GodfinderIcon instance;

	// Token: 0x04001EB9 RID: 7865
	public AudioSource audioPlayerPrefab;

	// Token: 0x04001EBA RID: 7866
	public AudioEvent showSound;

	// Token: 0x04001EBB RID: 7867
	private bool isVisible;

	// Token: 0x04001EBC RID: 7868
	private MeshRenderer renderer;

	// Token: 0x04001EBD RID: 7869
	private tk2dSpriteAnimator spriteAnimator;
}
