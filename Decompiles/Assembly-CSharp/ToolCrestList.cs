using System;
using UnityEngine;

// Token: 0x020005E0 RID: 1504
[CreateAssetMenu(fileName = "New Crest List", menuName = "Hornet/Tool Crest List")]
public class ToolCrestList : NamedScriptableObjectList<ToolCrest>
{
	// Token: 0x06003565 RID: 13669 RVA: 0x000EC62A File Offset: 0x000EA82A
	[ContextMenu("Unlock All", true)]
	public bool CanUnlockAll()
	{
		return Application.isPlaying;
	}

	// Token: 0x06003566 RID: 13670 RVA: 0x000EC634 File Offset: 0x000EA834
	[ContextMenu("Unlock All")]
	public void UnlockAll()
	{
		foreach (ToolCrest toolCrest in this)
		{
			toolCrest.Unlock();
		}
	}
}
