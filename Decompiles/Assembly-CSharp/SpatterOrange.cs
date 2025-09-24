using System;
using UnityEngine;

// Token: 0x02000559 RID: 1369
public class SpatterOrange : MonoBehaviour
{
	// Token: 0x060030F6 RID: 12534 RVA: 0x000D8D3A File Offset: 0x000D6F3A
	private void Start()
	{
		this.scaleModifier = Random.Range(this.scaleModifierMin, this.scaleModifierMax);
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x000D8D54 File Offset: 0x000D6F54
	private void OnEnable()
	{
		this.rb2d.isKinematic = false;
		this.circleCollider.enabled = true;
		this.idleTimer = 0f;
		this.animTimer = 0f;
		this.spriteRenderer.enabled = true;
		this.spriteRenderer.sprite = this.sprites[0];
		this.animFrame = 1;
		this.state = 0f;
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x000D8DC0 File Offset: 0x000D6FC0
	private void Update()
	{
		if (this.state == 0f)
		{
			this.FaceAngle();
			this.ProjectileSquash();
			this.idleTimer += Time.deltaTime;
			if (this.idleTimer > 3f)
			{
				this.Impact();
			}
		}
		if (this.state == 1f)
		{
			this.animTimer += Time.deltaTime;
			if (this.animTimer >= 1f / this.fps)
			{
				this.animTimer = 0f;
				this.animFrame++;
				if (this.animFrame > 6)
				{
					base.gameObject.Recycle();
				}
				else
				{
					this.spriteRenderer.sprite = this.sprites[this.animFrame];
				}
			}
		}
		if (this.state == 2f)
		{
			this.animTimer += Time.deltaTime;
			if (this.animTimer >= 0.25f)
			{
				base.gameObject.Recycle();
			}
		}
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x000D8EBC File Offset: 0x000D70BC
	private void Impact()
	{
		if (this.clipTable)
		{
			this.clipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		float num = Random.Range(this.splashScaleMin, this.splashScaleMax);
		base.transform.localScale = new Vector2(num, num);
		this.circleCollider.enabled = false;
		this.rb2d.isKinematic = true;
		this.rb2d.linearVelocity = new Vector2(0f, 0f);
		if (this.splashParticle)
		{
			this.splashParticle.main.startColor = this.spriteRenderer.color;
			this.splashParticle.Play();
			this.spriteRenderer.enabled = false;
			this.state = 2f;
			return;
		}
		this.spriteRenderer.sprite = this.sprites[1];
		this.state = 1f;
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x000D8FBC File Offset: 0x000D71BC
	private void FaceAngle()
	{
		Vector2 linearVelocity = this.rb2d.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x000D9008 File Offset: 0x000D7208
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

	// Token: 0x060030FC RID: 12540 RVA: 0x000D90B8 File Offset: 0x000D72B8
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		float x = safeContact.Normal.x;
		float y = safeContact.Normal.y;
		if (y == -1f)
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 180f);
		}
		else if (y == 1f)
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 0f);
		}
		else if (x == 1f)
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 270f);
		}
		else if (x == -1f)
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 90f);
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z + 90f);
		}
		this.Impact();
	}

	// Token: 0x060030FD RID: 12541 RVA: 0x000D9234 File Offset: 0x000D7434
	private void OnTriggerEnter2D()
	{
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 0f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		this.Impact();
	}

	// Token: 0x04003434 RID: 13364
	public Rigidbody2D rb2d;

	// Token: 0x04003435 RID: 13365
	public CircleCollider2D circleCollider;

	// Token: 0x04003436 RID: 13366
	public SpriteRenderer spriteRenderer;

	// Token: 0x04003437 RID: 13367
	public ParticleSystem splashParticle;

	// Token: 0x04003438 RID: 13368
	public RandomAudioClipTable clipTable;

	// Token: 0x04003439 RID: 13369
	public Sprite[] sprites;

	// Token: 0x0400343A RID: 13370
	private float stretchFactor = 1.7f;

	// Token: 0x0400343B RID: 13371
	private float stretchMinX = 0.6f;

	// Token: 0x0400343C RID: 13372
	private float stretchMaxY = 1.75f;

	// Token: 0x0400343D RID: 13373
	private float scaleModifier;

	// Token: 0x0400343E RID: 13374
	public float scaleModifierMin = 0.7f;

	// Token: 0x0400343F RID: 13375
	public float scaleModifierMax = 1.3f;

	// Token: 0x04003440 RID: 13376
	public float splashScaleMin = 1.5f;

	// Token: 0x04003441 RID: 13377
	public float splashScaleMax = 2f;

	// Token: 0x04003442 RID: 13378
	private float state;

	// Token: 0x04003443 RID: 13379
	public float fps = 30f;

	// Token: 0x04003444 RID: 13380
	private float idleTimer;

	// Token: 0x04003445 RID: 13381
	private float animTimer;

	// Token: 0x04003446 RID: 13382
	private int animFrame;
}
