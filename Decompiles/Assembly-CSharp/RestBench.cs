using System;
using UnityEngine;

// Token: 0x0200053D RID: 1341
public class RestBench : MonoBehaviour
{
	// Token: 0x06003023 RID: 12323 RVA: 0x000D47A8 File Offset: 0x000D29A8
	private void OnTriggerEnter2D(Collider2D otherObject)
	{
		HeroController component = otherObject.GetComponent<HeroController>();
		if (component)
		{
			component.NearBench(true);
		}
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x000D47CC File Offset: 0x000D29CC
	private void OnTriggerExit2D(Collider2D otherObject)
	{
		HeroController component = otherObject.GetComponent<HeroController>();
		if (!component)
		{
			return;
		}
		component.NearBench(false);
		ToolItemManager.ShowQueuedReminder();
	}
}
