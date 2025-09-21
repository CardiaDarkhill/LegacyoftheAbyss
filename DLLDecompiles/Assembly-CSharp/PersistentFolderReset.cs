using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class PersistentFolderReset : MonoBehaviour
{
	// Token: 0x06001BBA RID: 7098 RVA: 0x0008140C File Offset: 0x0007F60C
	private void Awake()
	{
		this.sourceFolder.SetActive(false);
		this.gm = GameManager.instance;
		this.gm.ResetSemiPersistentObjects += this.ResetSemiPersistentObjects;
		if (this.spawnFromPersonalPool)
		{
			this.spawnFromPersonalPool = base.gameObject.AddComponentIfNotPresent<PersonalObjectPool>();
			this.spawnedFolder = new GameObject(string.Format("{0} Spawn Folder", this)).transform;
			this.spawnedFolder.SetParent(this.sourceFolder.transform.parent, false);
			this.spawnedFolder.localPosition = this.sourceFolder.transform.localPosition;
			foreach (object obj in this.sourceFolder.transform)
			{
				GameObject gameObject = ((Transform)obj).gameObject;
				if (gameObject.activeSelf)
				{
					this.sourceList.Add(gameObject);
					PersonalObjectPool.EnsurePooledInScene(base.gameObject, gameObject, 3, false, true, false);
				}
			}
			PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		}
		this.SpawnNew();
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x00081540 File Offset: 0x0007F740
	private void OnDestroy()
	{
		if (this.gm)
		{
			this.gm.ResetSemiPersistentObjects -= this.ResetSemiPersistentObjects;
		}
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x00081568 File Offset: 0x0007F768
	private void ResetSemiPersistentObjects()
	{
		if (this.skipOnPlayerDeath)
		{
			HeroController instance = HeroController.instance;
			if (instance && instance.cState.dead)
			{
				return;
			}
		}
		this.SpawnNew();
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x000815A0 File Offset: 0x0007F7A0
	private void SpawnNew()
	{
		if (this.spawnFromPersonalPool)
		{
			foreach (GameObject gameObject in this.spawnedList)
			{
				if (gameObject != null && gameObject.activeSelf)
				{
					gameObject.Recycle();
				}
			}
			foreach (object obj in this.spawnedFolder)
			{
				((Transform)obj).gameObject.Recycle();
			}
			this.spawnedList.Clear();
			foreach (GameObject gameObject2 in this.sourceList)
			{
				Transform transform = gameObject2.transform;
				this.spawnedList.Add(gameObject2.Spawn(this.spawnedFolder, transform.localPosition, transform.localRotation));
			}
			return;
		}
		if (this.current)
		{
			Object.Destroy(this.current);
		}
		this.current = Object.Instantiate<GameObject>(this.sourceFolder, this.sourceFolder.transform.parent);
		this.current.SetActive(true);
	}

	// Token: 0x04001AC2 RID: 6850
	[SerializeField]
	private GameObject sourceFolder;

	// Token: 0x04001AC3 RID: 6851
	[SerializeField]
	private bool spawnFromPersonalPool;

	// Token: 0x04001AC4 RID: 6852
	[SerializeField]
	private int initialPoolSize = 3;

	// Token: 0x04001AC5 RID: 6853
	[SerializeField]
	private bool skipOnPlayerDeath;

	// Token: 0x04001AC6 RID: 6854
	private GameManager gm;

	// Token: 0x04001AC7 RID: 6855
	private GameObject current;

	// Token: 0x04001AC8 RID: 6856
	private Transform spawnedFolder;

	// Token: 0x04001AC9 RID: 6857
	private PersonalObjectPool personalObjectPool;

	// Token: 0x04001ACA RID: 6858
	private List<GameObject> sourceList = new List<GameObject>();

	// Token: 0x04001ACB RID: 6859
	private List<GameObject> spawnedList = new List<GameObject>();
}
