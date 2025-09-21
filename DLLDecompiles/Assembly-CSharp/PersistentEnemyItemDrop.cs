using System;
using UnityEngine;

// Token: 0x0200030F RID: 783
public class PersistentEnemyItemDrop : MonoBehaviour
{
	// Token: 0x06001BB4 RID: 7092 RVA: 0x00081158 File Offset: 0x0007F358
	private void Awake()
	{
		if (!this.dropPrefab)
		{
			return;
		}
		if (!this.item || !this.item.CanGetMore())
		{
			return;
		}
		this.healthManager = base.GetComponent<HealthManager>();
		if (!this.healthManager)
		{
			return;
		}
		this.enemyItemData = new PersistentItemData<bool>
		{
			ID = base.gameObject.name,
			SceneName = base.gameObject.scene.name
		};
		this.droppedItemID = string.Format("{0}_item_{1}", this.enemyItemData.ID, this.item.name);
		this.healthManager.StartedDead += delegate()
		{
			if (!SceneData.instance.PersistentBools.GetValueOrDefault(this.enemyItemData.SceneName, this.droppedItemID))
			{
				this.DropItem(false);
			}
		};
		if (!this.onlyOnStartedDead)
		{
			this.healthManager.OnDeath += delegate()
			{
				this.DropItem(true);
			};
		}
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x0008123C File Offset: 0x0007F43C
	private void DropItem(bool fling)
	{
		CollectableItemPickup collectableItemPickup = Object.Instantiate<CollectableItemPickup>(this.dropPrefab);
		if (!fling && this.startedDeadSpawnPoint)
		{
			collectableItemPickup.transform.SetPosition2D(this.startedDeadSpawnPoint.position);
		}
		else
		{
			collectableItemPickup.transform.SetPosition2D(base.transform.TransformPoint(this.healthManager.EffectOrigin));
		}
		collectableItemPickup.SetItem(this.item, false);
		collectableItemPickup.OnPickup.AddListener(delegate()
		{
			SceneData.instance.PersistentBools.SetValue(new PersistentItemData<bool>
			{
				ID = this.droppedItemID,
				SceneName = this.enemyItemData.SceneName,
				IsSemiPersistent = this.enemyItemData.IsSemiPersistent,
				Value = true
			});
		});
		if (fling)
		{
			float angleMin = (float)(this.healthManager.MegaFlingGeo ? 65 : 80);
			float angleMax = (float)(this.healthManager.MegaFlingGeo ? 115 : 100);
			float speedMin = (float)(this.healthManager.MegaFlingGeo ? 30 : 15);
			float speedMax = (float)(this.healthManager.MegaFlingGeo ? 45 : 30);
			FlingUtils.FlingObject(new FlingUtils.SelfConfig
			{
				Object = collectableItemPickup.gameObject,
				SpeedMin = speedMin,
				SpeedMax = speedMax,
				AngleMin = angleMin,
				AngleMax = angleMax
			}, base.transform, this.healthManager.EffectOrigin);
		}
	}

	// Token: 0x04001ABB RID: 6843
	[SerializeField]
	private CollectableItemPickup dropPrefab;

	// Token: 0x04001ABC RID: 6844
	[SerializeField]
	private SavedItem item;

	// Token: 0x04001ABD RID: 6845
	[SerializeField]
	private bool onlyOnStartedDead;

	// Token: 0x04001ABE RID: 6846
	[SerializeField]
	private Transform startedDeadSpawnPoint;

	// Token: 0x04001ABF RID: 6847
	private HealthManager healthManager;

	// Token: 0x04001AC0 RID: 6848
	private PersistentItemData<bool> enemyItemData;

	// Token: 0x04001AC1 RID: 6849
	private string droppedItemID;
}
