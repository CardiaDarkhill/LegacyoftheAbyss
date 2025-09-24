using System;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class EnemyHitEffectsGhost : MonoBehaviour, IHitEffectReciever
{
	// Token: 0x060019F3 RID: 6643 RVA: 0x000773AA File Offset: 0x000755AA
	protected void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x000773B8 File Offset: 0x000755B8
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
			this.spriteFlash.FlashEnemyHit();
		}
		GameObject gameObject = this.ghostHitPt.Spawn(base.transform.position + this.effectOrigin);
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			gameObject.transform.SetRotation2D(-22.5f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost1,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = -40f,
				AngleMax = 40f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost2,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = -40f,
				AngleMax = 40f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			break;
		case 1:
			gameObject.transform.SetRotation2D(70f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost1,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = 50f,
				AngleMax = 130f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost2,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = 50f,
				AngleMax = 130f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			break;
		case 2:
			gameObject.transform.SetRotation2D(160f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost1,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = 140f,
				AngleMax = 220f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost2,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = 140f,
				AngleMax = 220f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			break;
		case 3:
			gameObject.transform.SetRotation2D(-110f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost1,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = 230f,
				AngleMax = 310f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhost2,
				AmountMin = 2,
				AmountMax = 3,
				SpeedMin = 20f,
				SpeedMax = 35f,
				AngleMin = 230f,
				AngleMax = 310f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, this.effectOrigin, null, -1f);
			break;
		}
		this.didFireThisFrame = true;
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x000778DC File Offset: 0x00075ADC
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x040018E6 RID: 6374
	public Vector3 effectOrigin;

	// Token: 0x040018E7 RID: 6375
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x040018E8 RID: 6376
	public AudioEvent enemyDamage;

	// Token: 0x040018E9 RID: 6377
	[Space]
	public GameObject ghostHitPt;

	// Token: 0x040018EA RID: 6378
	public GameObject slashEffectGhost1;

	// Token: 0x040018EB RID: 6379
	public GameObject slashEffectGhost2;

	// Token: 0x040018EC RID: 6380
	private SpriteFlash spriteFlash;

	// Token: 0x040018ED RID: 6381
	private bool didFireThisFrame;
}
