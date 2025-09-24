using System;
using UnityEngine;

// Token: 0x020005DA RID: 1498
[Serializable]
public class PrefabSwapper : MonoBehaviour
{
	// Token: 0x0600352F RID: 13615 RVA: 0x000EBE14 File Offset: 0x000EA014
	public bool contains(string testGo)
	{
		string[] array = this.ignoreList;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == testGo)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04003895 RID: 14485
	public GameObject objToSwapout;

	// Token: 0x04003896 RID: 14486
	public string[] ignoreList;

	// Token: 0x04003897 RID: 14487
	public bool preserveZDepth = true;

	// Token: 0x04003898 RID: 14488
	public bool ignorePrefabs = true;
}
