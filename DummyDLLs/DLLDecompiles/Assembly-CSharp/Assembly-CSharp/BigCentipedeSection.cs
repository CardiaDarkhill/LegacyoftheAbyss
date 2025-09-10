using System;
using UnityEngine;

// Token: 0x0200029C RID: 668
public class BigCentipedeSection : MonoBehaviour
{
	// Token: 0x0600175C RID: 5980 RVA: 0x0006955E File Offset: 0x0006775E
	protected void Awake()
	{
		this.parent = base.GetComponentInParent<BigCentipede>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x00069578 File Offset: 0x00067778
	protected void Update()
	{
		Vector2 lhs = base.transform.position;
		if (!this.hasLeft)
		{
			if (Vector2.Dot(lhs, this.parent.Direction) > Vector2.Dot(this.parent.ExitPoint, this.parent.Direction))
			{
				this.meshRenderer.enabled = false;
				this.hasLeft = true;
				return;
			}
		}
		else if (Vector2.Dot(lhs, this.parent.Direction) > Vector2.Dot(this.parent.EntryPoint, this.parent.Direction) - 1.75f && Vector2.Dot(lhs, this.parent.Direction) < Vector2.Dot(this.parent.ExitPoint, this.parent.Direction))
		{
			this.meshRenderer.enabled = true;
			this.hasLeft = false;
		}
	}

	// Token: 0x04001607 RID: 5639
	private BigCentipede parent;

	// Token: 0x04001608 RID: 5640
	private MeshRenderer meshRenderer;

	// Token: 0x04001609 RID: 5641
	private Collider2D bodyCollider;

	// Token: 0x0400160A RID: 5642
	private bool hasLeft;
}
