using System;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class CorpseDropRegion : MonoBehaviour
{
	// Token: 0x06001839 RID: 6201 RVA: 0x0006E91C File Offset: 0x0006CB1C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Corpse component = collision.gameObject.GetComponent<Corpse>();
		if (component)
		{
			component.DropThroughFloor(this.waitToDrop);
		}
	}

	// Token: 0x04001714 RID: 5908
	[SerializeField]
	private bool waitToDrop;
}
