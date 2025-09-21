using System;
using UnityEngine;

// Token: 0x0200054B RID: 1355
public class SetGravityOnEnable : MonoBehaviour
{
	// Token: 0x06003070 RID: 12400 RVA: 0x000D62E5 File Offset: 0x000D44E5
	private void Start()
	{
		this.SetGravity();
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x000D62ED File Offset: 0x000D44ED
	private void OnEnable()
	{
		this.SetGravity();
	}

	// Token: 0x06003072 RID: 12402 RVA: 0x000D62F8 File Offset: 0x000D44F8
	private void SetGravity()
	{
		Rigidbody2D component = base.GetComponent<Rigidbody2D>();
		if (component)
		{
			component.gravityScale = this.gravity;
		}
	}

	// Token: 0x04003372 RID: 13170
	public float gravity;
}
