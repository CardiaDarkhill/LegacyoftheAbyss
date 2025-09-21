using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004B3 RID: 1203
public class ChildUpdateOrdered : MonoBehaviour, IUpdateBatchableLateUpdate, IUpdateBatchableUpdate
{
	// Token: 0x1700050F RID: 1295
	// (get) Token: 0x06002B72 RID: 11122 RVA: 0x000BE9CB File Offset: 0x000BCBCB
	public bool ShouldUpdate
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x000BE9D0 File Offset: 0x000BCBD0
	private void Awake()
	{
		this.childrenOrdered = new List<ChildUpdateOrdered.IUpdateOrderUpdate>();
		GameObject[] array = this.orderedParents;
		for (int i = 0; i < array.Length; i++)
		{
			ChildUpdateOrdered.IUpdateOrderUpdate[] componentsInChildren = array[i].GetComponentsInChildren<ChildUpdateOrdered.IUpdateOrderUpdate>(true);
			this.childrenOrdered.AddRange(componentsInChildren);
		}
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x000BEA13 File Offset: 0x000BCC13
	private void OnEnable()
	{
		this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		this.updateBatcher.Add(this);
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x000BEA31 File Offset: 0x000BCC31
	private void OnDisable()
	{
		if (this.updateBatcher)
		{
			this.updateBatcher.Remove(this);
			this.updateBatcher = null;
		}
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x000BEA54 File Offset: 0x000BCC54
	public void BatchedUpdate()
	{
		if (this.updateOrder != ChildUpdateOrdered.UpdateOrder.Update)
		{
			return;
		}
		this.DoUpdate();
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000BEA65 File Offset: 0x000BCC65
	public void BatchedLateUpdate()
	{
		if (this.updateOrder != ChildUpdateOrdered.UpdateOrder.LateUpdate)
		{
			return;
		}
		this.DoUpdate();
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x000BEA78 File Offset: 0x000BCC78
	private void DoUpdate()
	{
		foreach (ChildUpdateOrdered.IUpdateOrderUpdate updateOrderUpdate in this.childrenOrdered)
		{
			updateOrderUpdate.UpdateOrderUpdate();
		}
	}

	// Token: 0x04002CBE RID: 11454
	[SerializeField]
	private GameObject[] orderedParents;

	// Token: 0x04002CBF RID: 11455
	[SerializeField]
	private ChildUpdateOrdered.UpdateOrder updateOrder;

	// Token: 0x04002CC0 RID: 11456
	private UpdateBatcher updateBatcher;

	// Token: 0x04002CC1 RID: 11457
	private List<ChildUpdateOrdered.IUpdateOrderUpdate> childrenOrdered;

	// Token: 0x020017C7 RID: 6087
	public interface IUpdateOrderUpdate
	{
		// Token: 0x06008EA9 RID: 36521
		void UpdateOrderUpdate();
	}

	// Token: 0x020017C8 RID: 6088
	private enum UpdateOrder
	{
		// Token: 0x04008F5B RID: 36699
		Update,
		// Token: 0x04008F5C RID: 36700
		LateUpdate
	}
}
