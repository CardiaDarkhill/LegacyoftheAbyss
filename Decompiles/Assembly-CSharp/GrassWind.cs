using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class GrassWind : MonoBehaviour
{
	// Token: 0x06001510 RID: 5392 RVA: 0x0005F765 File Offset: 0x0005D965
	private void Awake()
	{
		this.col = base.GetComponent<Collider2D>();
		this.CacheGrassBehaviour();
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x0005F779 File Offset: 0x0005D979
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Nail Attack"))
		{
			if (this.dirty)
			{
				this.CacheGrassBehaviour();
			}
			if (this.hasGrassBehaviour)
			{
				base.StartCoroutine(this.DelayReact(this.grassBehaviour, collision));
			}
		}
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x0005F7B2 File Offset: 0x0005D9B2
	private IEnumerator DelayReact(GrassBehaviour behaviour, Collider2D collision)
	{
		yield return null;
		behaviour.WindReact(collision);
		yield break;
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x0005F7C8 File Offset: 0x0005D9C8
	private void OnTransformParentChanged()
	{
		this.dirty = true;
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x0005F7D1 File Offset: 0x0005D9D1
	private void CacheGrassBehaviour()
	{
		this.grassBehaviour = base.GetComponentInParent<GrassBehaviour>();
		this.hasGrassBehaviour = this.grassBehaviour;
	}

	// Token: 0x0400139E RID: 5022
	private Collider2D col;

	// Token: 0x0400139F RID: 5023
	private bool dirty;

	// Token: 0x040013A0 RID: 5024
	private bool hasGrassBehaviour;

	// Token: 0x040013A1 RID: 5025
	private GrassBehaviour grassBehaviour;
}
