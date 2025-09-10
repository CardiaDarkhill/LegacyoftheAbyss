using System;
using UnityEngine;

// Token: 0x0200054E RID: 1358
public class SilkPusher : MonoBehaviour
{
	// Token: 0x06003084 RID: 12420 RVA: 0x000D66CD File Offset: 0x000D48CD
	private void Awake()
	{
		this.polyCollider = base.GetComponent<PolygonCollider2D>();
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x000D66DB File Offset: 0x000D48DB
	private void OnEnable()
	{
		this.polyCollider.isTrigger = true;
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x000D66E9 File Offset: 0x000D48E9
	private void OnTriggerStay2D(Collider2D collision)
	{
		collision.transform.Translate(this.pushPerSecond * Time.deltaTime, 0f, 0f);
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x000D670C File Offset: 0x000D490C
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.polyCollider.isTrigger = false;
	}

	// Token: 0x0400338B RID: 13195
	private PolygonCollider2D polyCollider;

	// Token: 0x0400338C RID: 13196
	private bool touching;

	// Token: 0x0400338D RID: 13197
	public float pushPerSecond;
}
