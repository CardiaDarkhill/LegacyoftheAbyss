using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class InfectedEnemyEffects : MonoBehaviour, IHitEffectReciever
{
	// Token: 0x06001ADB RID: 6875 RVA: 0x0007D1A9 File Offset: 0x0007B3A9
	protected void Reset()
	{
		this.impactAudio.Reset();
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x0007D1B6 File Offset: 0x0007B3B6
	protected void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x0007D1C4 File Offset: 0x0007B3C4
	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (this.didFireThisFrame)
		{
			return;
		}
		if (this.spriteFlash != null)
		{
			this.spriteFlash.flashInfected();
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", true);
		this.impactAudio.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		this.hitFlashOrangePrefab.Spawn(base.transform.TransformPoint(this.effectOrigin));
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			if (!this.noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 3, 4, 10f, 15f, 120f, 150f, null, 0f);
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 8, 15, 10f, 25f, 30f, 60f, null, 0f);
			}
			this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(0f, 90f, 270f));
			break;
		case 1:
			if (!this.noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 8, 10, 20f, 30f, 80f, 100f, null, 0f);
			}
			this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(270f, 90f, 270f));
			break;
		case 2:
			if (!this.noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 3, 4, 10f, 15f, 30f, 60f, null, 0f);
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 8, 10, 15f, 25f, 120f, 150f, null, 0f);
			}
			this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(180f, 90f, 270f));
			break;
		case 3:
			if (!this.noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 4, 5, 15f, 25f, 140f, 180f, null, 0f);
				BloodSpawner.SpawnBlood(base.transform.position + this.effectOrigin, 4, 5, 15f, 25f, 360f, 400f, null, 0f);
			}
			this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(-72.5f, -180f, -180f));
			break;
		}
		this.didFireThisFrame = true;
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x0007D514 File Offset: 0x0007B714
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x040019F4 RID: 6644
	private SpriteFlash spriteFlash;

	// Token: 0x040019F5 RID: 6645
	[SerializeField]
	private Vector3 effectOrigin;

	// Token: 0x040019F6 RID: 6646
	[SerializeField]
	private AudioEvent impactAudio;

	// Token: 0x040019F7 RID: 6647
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x040019F8 RID: 6648
	[SerializeField]
	private GameObject hitFlashOrangePrefab;

	// Token: 0x040019F9 RID: 6649
	[SerializeField]
	private GameObject spatterOrangePrefab;

	// Token: 0x040019FA RID: 6650
	[SerializeField]
	private GameObject hitPuffPrefab;

	// Token: 0x040019FB RID: 6651
	[SerializeField]
	private bool noBlood;

	// Token: 0x040019FC RID: 6652
	private bool didFireThisFrame;
}
