using System;
using UnityEngine;

// Token: 0x020003D2 RID: 978
public class BreakItemsOnContact : MonoBehaviour
{
	// Token: 0x06002171 RID: 8561 RVA: 0x0009A930 File Offset: 0x00098B30
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.gameObject.layer != 17)
		{
			return;
		}
		IBreakOnContact component = collision.GetComponent<IBreakOnContact>();
		if (component != null)
		{
			component.Break();
			return;
		}
	}
}
