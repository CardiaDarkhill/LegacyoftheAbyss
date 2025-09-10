using System;
using UnityEngine;

// Token: 0x020002BC RID: 700
public sealed class SendEventOnEnable : MonoBehaviour
{
	// Token: 0x060018BB RID: 6331 RVA: 0x000716F9 File Offset: 0x0006F8F9
	private void Awake()
	{
		if (this.target == null)
		{
			this.target = base.GetComponentInParent<EventRelay>();
		}
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x00071715 File Offset: 0x0006F915
	private void OnValidate()
	{
		if (this.target == null)
		{
			this.target = base.GetComponentInParent<EventRelay>();
		}
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x00071731 File Offset: 0x0006F931
	private void OnEnable()
	{
		this.SendEvent();
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x00071739 File Offset: 0x0006F939
	public void SendEvent()
	{
		if (string.IsNullOrEmpty(this.eventName))
		{
			return;
		}
		if (this.target)
		{
			this.target.SendEvent(this.eventName);
		}
	}

	// Token: 0x040017B4 RID: 6068
	[SerializeField]
	private EventRelay target;

	// Token: 0x040017B5 RID: 6069
	[SerializeField]
	private string eventName;
}
