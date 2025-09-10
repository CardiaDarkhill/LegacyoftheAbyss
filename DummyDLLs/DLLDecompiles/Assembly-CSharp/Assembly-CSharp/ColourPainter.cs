using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class ColourPainter : MonoBehaviour
{
	// Token: 0x06001477 RID: 5239 RVA: 0x0005C4E8 File Offset: 0x0005A6E8
	private void Awake()
	{
		this.boxCollider = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x0005C4F8 File Offset: 0x0005A6F8
	private void Update()
	{
		if (this.active)
		{
			if (this.timer < this.delay)
			{
				this.timer += Time.deltaTime;
				return;
			}
			foreach (SpriteRenderer spriteRenderer in this.splatList)
			{
				spriteRenderer.color = this.colour;
			}
			this.boxCollider.enabled = false;
			this.active = false;
		}
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x0005C58C File Offset: 0x0005A78C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Extra Tag")
		{
			this.splatList.Add(collision.gameObject.GetComponent<SpriteRenderer>());
		}
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x0005C5B6 File Offset: 0x0005A7B6
	public void DoPaint()
	{
		this.splatList.Clear();
		this.timer = 0f;
		this.active = true;
		this.boxCollider.enabled = true;
	}

	// Token: 0x040012C7 RID: 4807
	public Color colour;

	// Token: 0x040012C8 RID: 4808
	public int chance;

	// Token: 0x040012C9 RID: 4809
	public float delay;

	// Token: 0x040012CA RID: 4810
	public List<SpriteRenderer> splatList;

	// Token: 0x040012CB RID: 4811
	private BoxCollider2D boxCollider;

	// Token: 0x040012CC RID: 4812
	private float timer;

	// Token: 0x040012CD RID: 4813
	private bool active;

	// Token: 0x040012CE RID: 4814
	private bool painted;
}
