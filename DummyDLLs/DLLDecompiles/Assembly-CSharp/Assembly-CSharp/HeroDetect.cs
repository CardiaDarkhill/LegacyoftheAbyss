using System;
using UnityEngine;

// Token: 0x020003C5 RID: 965
public class HeroDetect : MonoBehaviour
{
	// Token: 0x14000064 RID: 100
	// (add) Token: 0x060020A6 RID: 8358 RVA: 0x00096190 File Offset: 0x00094390
	// (remove) Token: 0x060020A7 RID: 8359 RVA: 0x000961C8 File Offset: 0x000943C8
	public event HeroDetect.ColliderEvent OnEnter;

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x060020A8 RID: 8360 RVA: 0x00096200 File Offset: 0x00094400
	// (remove) Token: 0x060020A9 RID: 8361 RVA: 0x00096238 File Offset: 0x00094438
	public event HeroDetect.ColliderEvent OnExit;

	// Token: 0x060020AA RID: 8362 RVA: 0x0009626D File Offset: 0x0009446D
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(collision);
		}
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x00096283 File Offset: 0x00094483
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.OnExit != null)
		{
			this.OnExit(collision);
		}
	}

	// Token: 0x0200167E RID: 5758
	// (Invoke) Token: 0x06008A36 RID: 35382
	public delegate void ColliderEvent(Collider2D collider);
}
