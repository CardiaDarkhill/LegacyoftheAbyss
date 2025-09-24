using System;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class QuickBurn : MonoBehaviour
{
	// Token: 0x060018B6 RID: 6326 RVA: 0x000713E0 File Offset: 0x0006F5E0
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody2D>();
		this.sprite = base.GetComponent<tk2dSprite>();
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		if (this.sprite)
		{
			this.startColour = this.sprite.color;
		}
		if (this.spriteRenderer)
		{
			this.startColour = this.spriteRenderer.color;
		}
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x0007144D File Offset: 0x0006F64D
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Coal"))
		{
			this.inCoal = true;
		}
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x00071468 File Offset: 0x0006F668
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.burnt || !this.inCoal || !collision.gameObject.CompareTag("Coal"))
		{
			return;
		}
		this.inCoal = false;
		this.timer = 0f;
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x000714A0 File Offset: 0x0006F6A0
	private void Update()
	{
		if (this.inCoal && !this.burnt)
		{
			this.timer += Time.deltaTime;
			if (this.timer >= 2f && !this.burnt)
			{
				this.burnt = true;
			}
		}
		if (this.burnt && !this.fading)
		{
			this.timer += Time.deltaTime;
			if (this.sprite)
			{
				this.sprite.color = Color.Lerp(this.startColour, this.burnColour, (this.timer - 2f) * 2f);
			}
			if (this.spriteRenderer)
			{
				this.spriteRenderer.color = Color.Lerp(this.startColour, this.burnColour, (this.timer - 2f) * 2f);
			}
			if (this.timer >= 2.5f && !this.fading)
			{
				this.fading = true;
				this.rb.isKinematic = true;
				this.rb.linearVelocity = new Vector2(0f, -1f);
			}
		}
		if (this.fading)
		{
			this.timer += Time.deltaTime;
			if (this.sprite)
			{
				this.sprite.color = Color.Lerp(this.burnColour, this.fadeColour, (this.timer - 2.5f) * 2f);
			}
			if (this.spriteRenderer)
			{
				this.spriteRenderer.color = Color.Lerp(this.burnColour, this.fadeColour, (this.timer - 2.5f) * 2f);
			}
			if (this.timer >= 3f)
			{
				base.transform.position = new Vector3(-50f, -50f, 0f);
			}
			if ((double)this.timer >= 3.5)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x040017AA RID: 6058
	private bool inCoal;

	// Token: 0x040017AB RID: 6059
	private bool burnt;

	// Token: 0x040017AC RID: 6060
	private bool fading;

	// Token: 0x040017AD RID: 6061
	private float timer;

	// Token: 0x040017AE RID: 6062
	private Rigidbody2D rb;

	// Token: 0x040017AF RID: 6063
	private tk2dSprite sprite;

	// Token: 0x040017B0 RID: 6064
	private SpriteRenderer spriteRenderer;

	// Token: 0x040017B1 RID: 6065
	private Color startColour;

	// Token: 0x040017B2 RID: 6066
	private readonly Color burnColour = new Color(0.075f, 0.06f, 0.06f, 1f);

	// Token: 0x040017B3 RID: 6067
	private readonly Color fadeColour = new Color(0.075f, 0.06f, 0.06f, 0f);
}
