using System;
using Ara;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class SoulThread : MonoBehaviour
{
	// Token: 0x060016A1 RID: 5793 RVA: 0x00065C58 File Offset: 0x00063E58
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody2D>();
		this.trailRenderer = base.GetComponent<AraTrail>();
		if (this.trailRenderer)
		{
			if (this.materials != null)
			{
				foreach (Material material in this.materials)
				{
					if (material)
					{
						Object.Destroy(material);
					}
				}
			}
			this.materials = new Material[this.trailRenderer.materials.Length];
			for (int j = 0; j < this.materials.Length; j++)
			{
				this.materials[j] = new Material(this.trailRenderer.materials[j]);
				this.materials[j].renderQueue = 4000;
			}
			this.trailRenderer.materials = this.materials;
		}
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x00065D24 File Offset: 0x00063F24
	private void OnDestroy()
	{
		if (this.materials != null)
		{
			foreach (Material material in this.materials)
			{
				if (material)
				{
					Object.Destroy(material);
				}
			}
		}
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x00065D60 File Offset: 0x00063F60
	private void OnEnable()
	{
		if (this.hero == null)
		{
			this.hero = GameManager.instance.hero_ctrl.transform;
		}
		this.speed = Random.Range(70f, 80f);
		this.angle = Random.Range(0f, 360f);
		this.turnSpeed = Random.Range(250f, 500f);
		this.timer = 0f;
		this.moving = true;
		this.trailRenderer.Clear();
		this.GetAngleToHero();
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x00065DF4 File Offset: 0x00063FF4
	private void Update()
	{
		if (this.moving)
		{
			this.GetAngleToHero();
			while (this.angleToHero < 0f)
			{
				this.angleToHero += 360f;
			}
			this.GetTurnDirection();
			if (this.clockwise)
			{
				this.angle -= this.turnSpeed * Time.deltaTime;
				if (this.angle < this.angleToHero && this.angleToHero - this.angle < 180f)
				{
					this.angle = this.angleToHero;
				}
			}
			else
			{
				this.angle += this.turnSpeed * Time.deltaTime;
				if (this.angle > this.angleToHero && this.angle - this.angleToHero < 180f)
				{
					this.angle = this.angleToHero;
				}
			}
			while (this.angle < 0f)
			{
				this.angle += 360f;
			}
			while (this.angle > 360f)
			{
				this.angle -= 360f;
			}
			this.turnSpeed += 3000f * Time.deltaTime;
			if (this.speed < 75f)
			{
				this.speed += 30f * Time.deltaTime;
			}
			this.ApplyMovement();
			if ((double)this.timer > 0.5)
			{
				this.StopThread();
			}
			if (Vector3.Distance(base.transform.position, this.hero.position) < 1f && this.timer > 0.1f)
			{
				this.StopThread();
			}
		}
		if (!this.moving && this.timer > 0.15f)
		{
			base.gameObject.Recycle();
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x00065FD8 File Offset: 0x000641D8
	private void GetAngleToHero()
	{
		float y = this.hero.position.y - base.transform.position.y;
		float x = this.hero.position.x - base.transform.position.x;
		this.angleToHero = Mathf.Atan2(y, x) * 57.295776f;
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x0006603C File Offset: 0x0006423C
	private void ApplyMovement()
	{
		float x = this.speed * Mathf.Cos(this.angle * 0.017453292f);
		float y = this.speed * Mathf.Sin(this.angle * 0.017453292f);
		Vector2 linearVelocity;
		linearVelocity.x = x;
		linearVelocity.y = y;
		this.rb.linearVelocity = linearVelocity;
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x00066098 File Offset: 0x00064298
	private void GetTurnDirection()
	{
		this.clockwise = true;
		if ((this.angleToHero > this.angle && this.angleToHero - this.angle < 180f) || (this.angleToHero < this.angle && this.angle - this.angleToHero > 180f))
		{
			this.clockwise = false;
		}
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x000660F7 File Offset: 0x000642F7
	private void StopThread()
	{
		this.moving = false;
		this.rb.linearVelocity = new Vector2(0f, 0f);
		this.timer = 0f;
	}

	// Token: 0x0400151C RID: 5404
	private Rigidbody2D rb;

	// Token: 0x0400151D RID: 5405
	private Transform hero;

	// Token: 0x0400151E RID: 5406
	private AraTrail trailRenderer;

	// Token: 0x0400151F RID: 5407
	public float angle;

	// Token: 0x04001520 RID: 5408
	public float angleToHero;

	// Token: 0x04001521 RID: 5409
	private float speed;

	// Token: 0x04001522 RID: 5410
	private float turnSpeed;

	// Token: 0x04001523 RID: 5411
	private float timer;

	// Token: 0x04001524 RID: 5412
	private bool moving;

	// Token: 0x04001525 RID: 5413
	private bool clockwise;

	// Token: 0x04001526 RID: 5414
	private Material[] materials;
}
