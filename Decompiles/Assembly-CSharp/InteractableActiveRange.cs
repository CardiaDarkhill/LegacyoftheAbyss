using System;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class InteractableActiveRange : MonoBehaviour, IInteractableBlocker
{
	// Token: 0x17000364 RID: 868
	// (get) Token: 0x06002109 RID: 8457 RVA: 0x00099410 File Offset: 0x00097610
	public bool IsBlocking
	{
		get
		{
			Vector2 vector = base.transform.position;
			return vector.x <= this.min.x || vector.x >= this.max.x || vector.y <= this.min.y || vector.y >= this.max.y;
		}
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x00099480 File Offset: 0x00097680
	private void OnDrawGizmosSelected()
	{
		Vector2 vector = this.max - this.min;
		Gizmos.DrawWireCube(this.min + vector / 2f, vector);
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x000994C8 File Offset: 0x000976C8
	private void OnValidate()
	{
		if (this.min.x > this.max.x)
		{
			this.min.x = this.max.x;
		}
		if (this.max.x < this.min.x)
		{
			this.max.x = this.min.x;
		}
		if (this.min.y > this.max.y)
		{
			this.min.y = this.max.y;
		}
		if (this.max.y < this.min.y)
		{
			this.max.y = this.min.y;
		}
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x0009958D File Offset: 0x0009778D
	private void OnEnable()
	{
		if (this.interactable)
		{
			this.interactable.AddBlocker(this);
		}
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x000995A8 File Offset: 0x000977A8
	private void OnDisable()
	{
		if (this.interactable)
		{
			this.interactable.RemoveBlocker(this);
		}
	}

	// Token: 0x04002016 RID: 8214
	[SerializeField]
	private InteractableBase interactable;

	// Token: 0x04002017 RID: 8215
	[SerializeField]
	private Vector2 min;

	// Token: 0x04002018 RID: 8216
	[SerializeField]
	private Vector2 max;
}
