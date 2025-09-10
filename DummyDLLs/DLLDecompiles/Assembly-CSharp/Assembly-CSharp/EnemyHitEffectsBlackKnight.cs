using System;
using UnityEngine;

// Token: 0x020002DB RID: 731
public class EnemyHitEffectsBlackKnight : MonoBehaviour, IHitEffectReciever
{
	// Token: 0x060019EF RID: 6639 RVA: 0x00077237 File Offset: 0x00075437
	protected void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x00077248 File Offset: 0x00075448
	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (this.didFireThisFrame)
		{
			return;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", true);
		this.enemyDamage.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		if (this.spriteFlash)
		{
			this.spriteFlash.flashInfected();
		}
		this.hitFlashOrange.Spawn(base.transform.position + this.effectOrigin);
		GameObject gameObject = this.hitPuffLarge.Spawn(base.transform.position + this.effectOrigin);
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			gameObject.transform.eulerAngles = new Vector3(0f, 90f, 270f);
			break;
		case 1:
			gameObject.transform.eulerAngles = new Vector3(270f, 90f, 270f);
			break;
		case 2:
			gameObject.transform.eulerAngles = new Vector3(180f, 90f, 270f);
			break;
		case 3:
			gameObject.transform.eulerAngles = new Vector3(-72.5f, -180f, -180f);
			break;
		}
		this.didFireThisFrame = true;
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x00077399 File Offset: 0x00075599
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x040018DF RID: 6367
	public Vector3 effectOrigin;

	// Token: 0x040018E0 RID: 6368
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x040018E1 RID: 6369
	public AudioEvent enemyDamage;

	// Token: 0x040018E2 RID: 6370
	[Space]
	public GameObject hitFlashOrange;

	// Token: 0x040018E3 RID: 6371
	public GameObject hitPuffLarge;

	// Token: 0x040018E4 RID: 6372
	private SpriteFlash spriteFlash;

	// Token: 0x040018E5 RID: 6373
	private bool didFireThisFrame;
}
