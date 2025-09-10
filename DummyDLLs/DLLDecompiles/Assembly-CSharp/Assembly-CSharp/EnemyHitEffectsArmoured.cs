using System;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class EnemyHitEffectsArmoured : MonoBehaviour, IHitEffectReciever
{
	// Token: 0x060019D7 RID: 6615 RVA: 0x00076C19 File Offset: 0x00074E19
	protected void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x00076C28 File Offset: 0x00074E28
	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (this.didFireThisFrame)
		{
			return;
		}
		this.enemyDamage.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		if (this.spriteFlash)
		{
			this.spriteFlash.flashArmoured();
		}
		GameObject gameObject = this.dustHit ? this.dustHit.Spawn(base.transform.position + this.effectOrigin) : null;
		if (gameObject)
		{
			gameObject.transform.SetPositionZ(-0.01f);
		}
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			if (gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(180f, 90f, 270f);
			}
			if (this.armourHit)
			{
				FSMUtility.SendEventToGameObject(this.armourHit, "ARMOUR HIT R", false);
			}
			break;
		case 1:
			if (gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(270f, 90f, 270f);
			}
			if (this.armourHit)
			{
				FSMUtility.SendEventToGameObject(this.armourHit, "ARMOUR HIT U", false);
			}
			break;
		case 2:
			if (gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(0f, 90f, 270f);
			}
			if (this.armourHit)
			{
				FSMUtility.SendEventToGameObject(this.armourHit, "ARMOUR HIT L", false);
			}
			break;
		case 3:
			if (gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(-72.5f, -180f, -180f);
			}
			if (this.armourHit)
			{
				FSMUtility.SendEventToGameObject(this.armourHit, "ARMOUR HIT D", false);
			}
			break;
		}
		this.didFireThisFrame = true;
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x00076E12 File Offset: 0x00075012
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x040018C9 RID: 6345
	public Vector3 effectOrigin;

	// Token: 0x040018CA RID: 6346
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x040018CB RID: 6347
	public AudioEvent enemyDamage;

	// Token: 0x040018CC RID: 6348
	[Space]
	public GameObject dustHit;

	// Token: 0x040018CD RID: 6349
	public GameObject armourHit;

	// Token: 0x040018CE RID: 6350
	private SpriteFlash spriteFlash;

	// Token: 0x040018CF RID: 6351
	private bool didFireThisFrame;
}
