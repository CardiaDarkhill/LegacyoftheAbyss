using System;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
public sealed class ToolUsageCounter : MonoBehaviour
{
	// Token: 0x0600367D RID: 13949 RVA: 0x000F07D1 File Offset: 0x000EE9D1
	private void OnEnable()
	{
		if (!ObjectPool.IsCreatingPool && this.toolItem != null)
		{
			this.toolItem.CustomUsage(this.usageAmount);
		}
	}

	// Token: 0x0400395E RID: 14686
	[SerializeField]
	private ToolItem toolItem;

	// Token: 0x0400395F RID: 14687
	[SerializeField]
	private int usageAmount;
}
