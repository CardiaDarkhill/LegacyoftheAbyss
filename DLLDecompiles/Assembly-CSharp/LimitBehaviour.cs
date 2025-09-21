using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class LimitBehaviour : MonoBehaviour
{
	// Token: 0x06001B7E RID: 7038 RVA: 0x000806C8 File Offset: 0x0007E8C8
	private void OnDisable()
	{
		this.RemoveSelf();
		if (LimitBehaviour.behaviourLists.Count > 0)
		{
			bool flag = true;
			foreach (KeyValuePair<string, List<GameObject>> keyValuePair in LimitBehaviour.behaviourLists)
			{
				if (keyValuePair.Value.Count > 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				LimitBehaviour.behaviourLists.Clear();
			}
		}
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x00080748 File Offset: 0x0007E948
	public void Add()
	{
		if (this.id != "")
		{
			List<GameObject> list;
			if (!LimitBehaviour.behaviourLists.ContainsKey(this.id))
			{
				list = new List<GameObject>();
				LimitBehaviour.behaviourLists.Add(this.id, list);
			}
			else
			{
				list = LimitBehaviour.behaviourLists[this.id];
			}
			if (!list.Contains(base.gameObject))
			{
				list.Add(base.gameObject);
				if (list.Count > 5)
				{
					this.RemoveFirst();
				}
			}
		}
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000807D0 File Offset: 0x0007E9D0
	public void RemoveFirst()
	{
		if (this.id != "" && LimitBehaviour.behaviourLists.ContainsKey(this.id))
		{
			List<GameObject> list = LimitBehaviour.behaviourLists[this.id];
			GameObject go = list[0];
			list.RemoveAt(0);
			FSMUtility.SendEventToGameObject(go, this.forceRemoveEvent, false);
		}
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x0008082C File Offset: 0x0007EA2C
	public void RemoveSelf()
	{
		if (this.id != "" && LimitBehaviour.behaviourLists.ContainsKey(this.id) && LimitBehaviour.behaviourLists[this.id].Contains(base.gameObject))
		{
			LimitBehaviour.behaviourLists[this.id].Remove(base.gameObject);
		}
	}

	// Token: 0x04001A85 RID: 6789
	public static Dictionary<string, List<GameObject>> behaviourLists = new Dictionary<string, List<GameObject>>();

	// Token: 0x04001A86 RID: 6790
	public string id = "";

	// Token: 0x04001A87 RID: 6791
	public int limit = 5;

	// Token: 0x04001A88 RID: 6792
	public string forceRemoveEvent = "REMOVE";
}
