using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200053E RID: 1342
public abstract class Rigidbody2DDisturberBase : MonoBehaviour
{
	// Token: 0x06003026 RID: 12326 RVA: 0x000D4800 File Offset: 0x000D2A00
	protected virtual void Awake()
	{
		if (this.bodies == null || this.bodies.Length == 0)
		{
			this.bodies = (from body in base.GetComponentsInChildren<Rigidbody2D>()
			where !body.transform.parent || !body.transform.parent.GetComponentInParent<Rigidbody2D>()
			select body).ToArray<Rigidbody2D>();
		}
	}

	// Token: 0x040032F9 RID: 13049
	[SerializeField]
	protected Rigidbody2D[] bodies;
}
