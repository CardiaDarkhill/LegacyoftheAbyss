using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003FF RID: 1023
public class LimitSendEvents : MonoBehaviour
{
	// Token: 0x060022C3 RID: 8899 RVA: 0x0009FB09 File Offset: 0x0009DD09
	private void OnEnable()
	{
		this.sentList.Clear();
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x0009FB18 File Offset: 0x0009DD18
	private void Update()
	{
		if (this.monitorCollider)
		{
			bool enabled = this.monitorCollider.enabled;
			bool? flag = this.previousColliderState;
			if (enabled == flag.GetValueOrDefault() & flag != null)
			{
				return;
			}
			this.previousColliderState = new bool?(this.monitorCollider.enabled);
		}
		if (this.sentList.Count > 0)
		{
			this.sentList.Clear();
		}
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x0009FB87 File Offset: 0x0009DD87
	public bool Add(GameObject obj)
	{
		if (!this.sentList.Contains(obj))
		{
			this.sentList.Add(obj);
			return true;
		}
		return false;
	}

	// Token: 0x04002193 RID: 8595
	public Collider2D monitorCollider;

	// Token: 0x04002194 RID: 8596
	private bool? previousColliderState;

	// Token: 0x04002195 RID: 8597
	private List<GameObject> sentList = new List<GameObject>();
}
