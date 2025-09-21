using System;
using UnityEngine;

// Token: 0x02000558 RID: 1368
public class SpatterHoney : MonoBehaviour
{
	// Token: 0x060030EE RID: 12526 RVA: 0x000D8B6B File Offset: 0x000D6D6B
	private void Start()
	{
		this.scaleModifier = Random.Range(this.scaleModifierMin, this.scaleModifierMax);
	}

	// Token: 0x060030EF RID: 12527 RVA: 0x000D8B84 File Offset: 0x000D6D84
	private void OnEnable()
	{
		this.rb2d.isKinematic = false;
		this.circleCollider.enabled = true;
		this.idleTimer = 0f;
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x000D8BA9 File Offset: 0x000D6DA9
	private void Update()
	{
		this.FaceAngle();
		this.ProjectileSquash();
		this.idleTimer += Time.deltaTime;
		if (this.idleTimer > 3f)
		{
			this.Impact();
		}
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x000D8BDC File Offset: 0x000D6DDC
	private void Impact()
	{
		if (this.idleTimer > 0.1f)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x000D8BF8 File Offset: 0x000D6DF8
	private void FaceAngle()
	{
		Vector2 linearVelocity = this.rb2d.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x000D8C44 File Offset: 0x000D6E44
	private void ProjectileSquash()
	{
		float num = 1f - this.rb2d.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		float num2 = 1f + this.rb2d.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		if (num2 < this.stretchMinX)
		{
			num2 = this.stretchMinX;
		}
		if (num > this.stretchMaxY)
		{
			num = this.stretchMaxY;
		}
		num *= this.scaleModifier;
		num2 *= this.scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x000D8CF3 File Offset: 0x000D6EF3
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.Impact();
	}

	// Token: 0x04003429 RID: 13353
	public Rigidbody2D rb2d;

	// Token: 0x0400342A RID: 13354
	public CircleCollider2D circleCollider;

	// Token: 0x0400342B RID: 13355
	public SpriteRenderer spriteRenderer;

	// Token: 0x0400342C RID: 13356
	private float stretchFactor = 1.4f;

	// Token: 0x0400342D RID: 13357
	private float stretchMinX = 0.7f;

	// Token: 0x0400342E RID: 13358
	private float stretchMaxY = 1.75f;

	// Token: 0x0400342F RID: 13359
	private float scaleModifier;

	// Token: 0x04003430 RID: 13360
	public float scaleModifierMin = 0.7f;

	// Token: 0x04003431 RID: 13361
	public float scaleModifierMax = 1.3f;

	// Token: 0x04003432 RID: 13362
	private float state;

	// Token: 0x04003433 RID: 13363
	private float idleTimer;
}
