using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020006EE RID: 1774
public class MeshSortingOrder : MonoBehaviour
{
	// Token: 0x06003FB2 RID: 16306 RVA: 0x00119048 File Offset: 0x00117248
	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(this.layerName))
		{
			this.layerID = SortingLayer.NameToID(this.layerName);
			this.layerName = null;
		}
		this.rend = base.GetComponent<MeshRenderer>();
		this.rend.sortingLayerID = this.layerID;
		this.rend.sortingOrder = this.order;
	}

	// Token: 0x06003FB3 RID: 16307 RVA: 0x001190A8 File Offset: 0x001172A8
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06003FB4 RID: 16308 RVA: 0x001190B0 File Offset: 0x001172B0
	private void OnEnable()
	{
		ComponentSingleton<MeshSortingOrderCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
	}

	// Token: 0x06003FB5 RID: 16309 RVA: 0x001190C8 File Offset: 0x001172C8
	private void OnDisable()
	{
		ComponentSingleton<MeshSortingOrderCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
	}

	// Token: 0x06003FB6 RID: 16310 RVA: 0x001190E0 File Offset: 0x001172E0
	private void OnUpdate()
	{
		if (this.rend.sortingLayerID != this.layerID)
		{
			this.rend.sortingLayerID = this.layerID;
		}
		if (this.rend.sortingOrder != this.order)
		{
			this.rend.sortingOrder = this.order;
		}
	}

	// Token: 0x0400415A RID: 16730
	[SerializeField]
	[HideInInspector]
	private string layerName;

	// Token: 0x0400415B RID: 16731
	[SerializeField]
	[SortingLayer]
	private int layerID;

	// Token: 0x0400415C RID: 16732
	[SerializeField]
	private int order;

	// Token: 0x0400415D RID: 16733
	private MeshRenderer rend;
}
