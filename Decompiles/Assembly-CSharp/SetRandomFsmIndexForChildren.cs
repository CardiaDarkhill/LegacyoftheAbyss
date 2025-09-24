using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000369 RID: 873
public class SetRandomFsmIndexForChildren : MonoBehaviour
{
	// Token: 0x06001E09 RID: 7689 RVA: 0x0008ACD0 File Offset: 0x00088ED0
	private void Awake()
	{
		if (!this.root || string.IsNullOrEmpty(this.fsmIntName))
		{
			return;
		}
		List<FsmInt> list = new List<FsmInt>();
		foreach (object obj in this.root)
		{
			PlayMakerFSM component = ((Transform)obj).GetComponent<PlayMakerFSM>();
			if (component)
			{
				FsmInt fsmInt = component.FsmVariables.FindFsmInt(this.fsmIntName);
				if (fsmInt != null)
				{
					list.Add(fsmInt);
				}
			}
		}
		list.Shuffle<FsmInt>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value = this.startIndex + i;
		}
	}

	// Token: 0x04001D23 RID: 7459
	[SerializeField]
	private Transform root;

	// Token: 0x04001D24 RID: 7460
	[SerializeField]
	private string fsmIntName;

	// Token: 0x04001D25 RID: 7461
	[SerializeField]
	private int startIndex;
}
