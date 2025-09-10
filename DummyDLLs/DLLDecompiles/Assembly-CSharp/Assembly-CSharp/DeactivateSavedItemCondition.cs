using System;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class DeactivateSavedItemCondition : MonoBehaviour
{
	// Token: 0x06001372 RID: 4978 RVA: 0x00058AC9 File Offset: 0x00056CC9
	private void Start()
	{
		this.hasStarted = true;
		this.Evaluate();
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x00058AD8 File Offset: 0x00056CD8
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.Evaluate();
		}
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x00058AE8 File Offset: 0x00056CE8
	public void Evaluate()
	{
		base.gameObject.SetActive(this.item && this.item.CanGetMore());
	}

	// Token: 0x040011DD RID: 4573
	[SerializeField]
	private SavedItem item;

	// Token: 0x040011DE RID: 4574
	private bool hasStarted;
}
