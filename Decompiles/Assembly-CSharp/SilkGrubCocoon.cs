using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200054D RID: 1357
public class SilkGrubCocoon : MonoBehaviour
{
	// Token: 0x17000552 RID: 1362
	// (get) Token: 0x06003079 RID: 12409 RVA: 0x000D6388 File Offset: 0x000D4588
	public static bool IsAnyActive
	{
		get
		{
			foreach (SilkGrubCocoon silkGrubCocoon in SilkGrubCocoon._cocoons)
			{
				if (!silkGrubCocoon.isBroken && silkGrubCocoon.grubSingRange && silkGrubCocoon.grubSingRange.IsInside)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x000D63FC File Offset: 0x000D45FC
	private void Awake()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isBroken;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isBroken = value;
				if (this.isBroken)
				{
					this.SetBroken();
				}
			};
		}
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x000D6439 File Offset: 0x000D4639
	private void OnEnable()
	{
		SilkGrubCocoon._cocoons.Add(this);
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x000D6446 File Offset: 0x000D4646
	private void OnDisable()
	{
		SilkGrubCocoon._cocoons.Remove(this);
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x000D6454 File Offset: 0x000D4654
	private void Start()
	{
		this.hitsLeft = this.hitsToBreak;
		if (this.enableOnBreak)
		{
			this.enableOnBreak.SetActive(false);
		}
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x000D647B File Offset: 0x000D467B
	private void SetBroken()
	{
		this.isBroken = true;
		if (this.persistent)
		{
			this.persistent.SaveState();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x000D64A8 File Offset: 0x000D46A8
	public void WasHit()
	{
		if (this.isBroken)
		{
			return;
		}
		Transform transform = this.hitEffectSpawnPoint ? this.hitEffectSpawnPoint : base.transform;
		Vector3 position = transform.position;
		HeroController.instance.SilkGain();
		this.hitEffectPrefabs.SpawnAll(position);
		this.hitsLeft--;
		if (this.hitsLeft > 0)
		{
			BloodSpawner.SpawnBlood(this.hitBlood, transform, null);
			this.OnHit.Invoke();
			return;
		}
		BloodSpawner.SpawnBlood(this.breakBlood, transform, null);
		CollectableItemPickup collectableItemPickup = this.dropItemPrefab ? this.dropItemPrefab : Gameplay.CollectableItemPickupInstantPrefab;
		if (collectableItemPickup && this.dropItem)
		{
			Transform transform2 = this.dropItemSpawnPoint ? this.dropItemSpawnPoint : base.transform;
			CollectableItemPickup collectableItemPickup2 = Object.Instantiate<CollectableItemPickup>(collectableItemPickup);
			collectableItemPickup2.transform.SetPosition2D(transform2.position);
			collectableItemPickup2.SetItem(this.dropItem, false);
			FlingUtils.FlingObject(this.dropItemFling.GetSelfConfig(collectableItemPickup2.gameObject), transform2, Vector2.zero);
		}
		this.breakCameraShake.DoShake(this, true);
		if (this.enableOnBreak)
		{
			this.enableOnBreak.transform.SetParent(null, true);
			this.enableOnBreak.SetActive(true);
		}
		this.breakEffectPrefabs.SpawnAll(position);
		if (this.setPDBoolOnBreak != "")
		{
			GameManager.instance.playerData.SetBool(this.setPDBoolOnBreak, true);
		}
		if (this.unsetPDBoolOnBreak != "")
		{
			GameManager.instance.playerData.SetBool(this.unsetPDBoolOnBreak, false);
		}
		NoiseMaker component = base.GetComponent<NoiseMaker>();
		if (component)
		{
			component.CreateNoise();
		}
		this.SetBroken();
	}

	// Token: 0x04003375 RID: 13173
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003376 RID: 13174
	[SerializeField]
	private TrackTriggerObjects grubSingRange;

	// Token: 0x04003377 RID: 13175
	[SerializeField]
	private Transform hitEffectSpawnPoint;

	// Token: 0x04003378 RID: 13176
	[SerializeField]
	private GameObject[] hitEffectPrefabs;

	// Token: 0x04003379 RID: 13177
	[SerializeField]
	private GameObject enableOnBreak;

	// Token: 0x0400337A RID: 13178
	[SerializeField]
	private GameObject[] breakEffectPrefabs;

	// Token: 0x0400337B RID: 13179
	[SerializeField]
	private BloodSpawner.Config hitBlood;

	// Token: 0x0400337C RID: 13180
	[SerializeField]
	private BloodSpawner.Config breakBlood;

	// Token: 0x0400337D RID: 13181
	[SerializeField]
	private CameraShakeTarget breakCameraShake;

	// Token: 0x0400337E RID: 13182
	[SerializeField]
	private int hitsToBreak;

	// Token: 0x0400337F RID: 13183
	[SerializeField]
	private CollectableItem dropItem;

	// Token: 0x04003380 RID: 13184
	[SerializeField]
	private CollectableItemPickup dropItemPrefab;

	// Token: 0x04003381 RID: 13185
	[SerializeField]
	private Transform dropItemSpawnPoint;

	// Token: 0x04003382 RID: 13186
	[SerializeField]
	private FlingUtils.ObjectFlingParams dropItemFling;

	// Token: 0x04003383 RID: 13187
	[SerializeField]
	private GameObject soulThreadPrefab;

	// Token: 0x04003384 RID: 13188
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string setPDBoolOnBreak;

	// Token: 0x04003385 RID: 13189
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string unsetPDBoolOnBreak;

	// Token: 0x04003386 RID: 13190
	[Space]
	public UnityEvent OnHit;

	// Token: 0x04003387 RID: 13191
	private int hitsLeft;

	// Token: 0x04003388 RID: 13192
	private bool isBroken;

	// Token: 0x04003389 RID: 13193
	private float addSilkTimer;

	// Token: 0x0400338A RID: 13194
	private static readonly List<SilkGrubCocoon> _cocoons = new List<SilkGrubCocoon>();
}
