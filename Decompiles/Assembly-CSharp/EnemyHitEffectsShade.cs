using System;
using UnityEngine;

// Token: 0x020002DF RID: 735
public class EnemyHitEffectsShade : MonoBehaviour, IHitEffectReciever
{
	// Token: 0x06001A16 RID: 6678 RVA: 0x00077EA4 File Offset: 0x000760A4
	private void Awake()
	{
		this.sprite = base.GetComponent<tk2dSprite>();
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x00077EB4 File Offset: 0x000760B4
	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (this.didFireThisFrame)
		{
			return;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", true);
		this.hollowShadeStartled.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		this.heroDamage.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		this.sprite.color = Color.black;
		base.SendMessage("ColorReturnNeutral");
		this.hitFlashBlack.Spawn(base.transform.position + this.effectOrigin);
		GameObject gameObject = this.hitShade.Spawn(base.transform.position + this.effectOrigin);
		float minInclusive = 1f;
		float maxInclusive = 1f;
		float minInclusive2 = 0f;
		float maxInclusive2 = 360f;
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			gameObject.transform.eulerAngles = new Vector3(0f, 90f, 0f);
			minInclusive = 1f;
			maxInclusive = 1.75f;
			minInclusive2 = -30f;
			maxInclusive2 = 30f;
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhostDark1,
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
				Prefab = this.slashEffectGhostDark2,
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
			gameObject.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
			minInclusive = 1f;
			maxInclusive = 1.75f;
			minInclusive2 = 60f;
			maxInclusive2 = 120f;
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhostDark1,
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
				Prefab = this.slashEffectGhostDark2,
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
			gameObject.transform.eulerAngles = new Vector3(0f, -90f, 0f);
			minInclusive = -1f;
			maxInclusive = -1.75f;
			minInclusive2 = -30f;
			maxInclusive2 = 30f;
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhostDark1,
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
				Prefab = this.slashEffectGhostDark2,
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
			gameObject.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
			minInclusive = 1f;
			maxInclusive = 1.75f;
			minInclusive2 = -60f;
			maxInclusive2 = -120f;
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.slashEffectGhostDark1,
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
				Prefab = this.slashEffectGhostDark2,
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
		for (int i = 0; i < 3; i++)
		{
			GameObject gameObject2 = this.slashEffectShade.Spawn(base.transform.position + this.effectOrigin);
			gameObject2.transform.SetScaleX(Random.Range(minInclusive, maxInclusive));
			gameObject2.transform.SetRotation2D(Random.Range(minInclusive2, maxInclusive2));
		}
		this.didFireThisFrame = true;
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x00078533 File Offset: 0x00076733
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x04001902 RID: 6402
	public Vector3 effectOrigin;

	// Token: 0x04001903 RID: 6403
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x04001904 RID: 6404
	public AudioEvent hollowShadeStartled;

	// Token: 0x04001905 RID: 6405
	public AudioEvent heroDamage;

	// Token: 0x04001906 RID: 6406
	[Space]
	public GameObject hitFlashBlack;

	// Token: 0x04001907 RID: 6407
	public GameObject hitShade;

	// Token: 0x04001908 RID: 6408
	public GameObject slashEffectGhostDark1;

	// Token: 0x04001909 RID: 6409
	public GameObject slashEffectGhostDark2;

	// Token: 0x0400190A RID: 6410
	public GameObject slashEffectShade;

	// Token: 0x0400190B RID: 6411
	private tk2dSprite sprite;

	// Token: 0x0400190C RID: 6412
	private bool didFireThisFrame;
}
