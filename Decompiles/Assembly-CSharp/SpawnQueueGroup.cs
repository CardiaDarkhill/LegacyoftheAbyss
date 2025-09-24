using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F7 RID: 503
[CreateAssetMenu(menuName = "Hornet/Spawn Queue Group")]
public class SpawnQueueGroup : ScriptableObject
{
	// Token: 0x06001348 RID: 4936 RVA: 0x00058544 File Offset: 0x00056744
	public void AddSpawned(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		if (this.recycleQueue == null)
		{
			this.recycleQueue = new List<GameObject>(this.groupLimit);
		}
		if (this.recycleQueue.Count >= this.groupLimit)
		{
			GameObject gameObject = this.recycleQueue[0];
			this.recycleQueue.RemoveAt(0);
			gameObject.SendMessage("OnSpawnDequeued", SendMessageOptions.DontRequireReceiver);
			FSMUtility.SendEventToGameObject(gameObject, "SPAWN DEQUEUED", false);
		}
		RecycleResetHandler.Add(obj, delegate()
		{
			this.recycleQueue.Remove(obj);
		});
		this.recycleQueue.Add(obj);
	}

	// Token: 0x040011B6 RID: 4534
	private const string QUEUE_POP_FSM_EVENT = "SPAWN DEQUEUED";

	// Token: 0x040011B7 RID: 4535
	private const string QUEUE_POP_SCRIPT_MESSAGE = "OnSpawnDequeued";

	// Token: 0x040011B8 RID: 4536
	[SerializeField]
	private int groupLimit;

	// Token: 0x040011B9 RID: 4537
	[NonSerialized]
	private List<GameObject> recycleQueue;
}
