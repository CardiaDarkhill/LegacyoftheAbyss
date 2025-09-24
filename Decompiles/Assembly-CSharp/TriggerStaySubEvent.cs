using System;
using UnityEngine;

// Token: 0x02000601 RID: 1537
public sealed class TriggerStaySubEvent : TriggerSubEvent
{
	// Token: 0x140000B1 RID: 177
	// (add) Token: 0x060036F1 RID: 14065 RVA: 0x000F24B4 File Offset: 0x000F06B4
	// (remove) Token: 0x060036F2 RID: 14066 RVA: 0x000F24EC File Offset: 0x000F06EC
	public event TriggerSubEvent.CollisionEvent OnTriggerStayed;

	// Token: 0x060036F3 RID: 14067 RVA: 0x000F2521 File Offset: 0x000F0721
	private void OnTriggerStay2D(Collider2D other)
	{
		TriggerSubEvent.CollisionEvent onTriggerStayed = this.OnTriggerStayed;
		if (onTriggerStayed == null)
		{
			return;
		}
		onTriggerStayed(other);
	}
}
