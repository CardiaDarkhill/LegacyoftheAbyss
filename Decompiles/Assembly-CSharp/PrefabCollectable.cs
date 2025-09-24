using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Prefab)")]
public class PrefabCollectable : FakeCollectable, ISavedItemPreSpawn, IPreSpawn
{
	// Token: 0x17000212 RID: 530
	// (get) Token: 0x060012BD RID: 4797 RVA: 0x00056A54 File Offset: 0x00054C54
	public override bool CanGetMultipleAtOnce
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x00056A57 File Offset: 0x00054C57
	public override bool GetTakesHeroControl()
	{
		return !this.prefab.GetComponent<SilkRationObject>();
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00056A6C File Offset: 0x00054C6C
	public override void Get(bool showPopup = true)
	{
		base.Get(showPopup);
		SilkRationObject component = this.prefab.GetComponent<SilkRationObject>();
		if (component)
		{
			component.AddSilk();
			if (showPopup)
			{
				component.CollectPopup();
				return;
			}
		}
		else
		{
			this.Spawn();
		}
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00056AAB File Offset: 0x00054CAB
	public GameObject Spawn()
	{
		if (!this.prefab)
		{
			return null;
		}
		if (this.spawnPrefab)
		{
			return this.prefab.Spawn();
		}
		return Object.Instantiate<GameObject>(this.prefab);
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00056ADB File Offset: 0x00054CDB
	public void PreSpawnGet(bool showPopup = true)
	{
		base.Get(showPopup);
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00056AE4 File Offset: 0x00054CE4
	public bool TryGetPrespawnedItem(out PreSpawnedItem item)
	{
		if (this.spawnPrefab || this.prefab == null || !this.canPreSpawn)
		{
			item = null;
			return false;
		}
		if (this.prefab.GetComponent<SilkRationObject>())
		{
			item = null;
			return false;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
		item = new PreSpawnedItem(gameObject, false);
		gameObject.gameObject.SetActive(false);
		return true;
	}

	// Token: 0x04001178 RID: 4472
	[Space]
	[SerializeField]
	private GameObject prefab;

	// Token: 0x04001179 RID: 4473
	[SerializeField]
	private bool spawnPrefab;

	// Token: 0x0400117A RID: 4474
	[SerializeField]
	private bool canPreSpawn = true;
}
