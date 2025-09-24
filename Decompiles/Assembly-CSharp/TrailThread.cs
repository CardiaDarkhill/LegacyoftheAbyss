using System;
using Ara;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class TrailThread : MonoBehaviour
{
	// Token: 0x060016C8 RID: 5832 RVA: 0x000666D0 File Offset: 0x000648D0
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

	// Token: 0x060016C9 RID: 5833 RVA: 0x0006679C File Offset: 0x0006499C
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

	// Token: 0x060016CA RID: 5834 RVA: 0x000667D8 File Offset: 0x000649D8
	private void OnEnable()
	{
		this.speed = Random.Range(this.speedMin, this.speedMax);
		this.angle = Random.Range(0f, 360f);
		this.turnSpeed = 0f;
		this.turnAccel = Random.Range(0f, this.turnAccelMax);
		if (Random.Range(1, 100) > 50)
		{
			this.turnAccel *= -1f;
		}
		this.timer = this.trailStartPause;
		this.trailRenderer.Clear();
		this.trailRenderer.enabled = false;
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x00066874 File Offset: 0x00064A74
	private void Update()
	{
		this.angle += this.turnSpeed * Time.deltaTime;
		while (this.angle < 0f)
		{
			this.angle += 360f;
		}
		while (this.angle > 360f)
		{
			this.angle -= 360f;
		}
		this.turnSpeed += this.turnAccel * Time.deltaTime;
		if (this.angle > this.turnMax)
		{
			this.angle = this.turnMax;
		}
		this.ApplyMovement();
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.trailRenderer.enabled = true;
			}
		}
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x00066950 File Offset: 0x00064B50
	private void ApplyMovement()
	{
		float x = this.speed * Mathf.Cos(this.angle * 0.017453292f);
		float y = this.speed * Mathf.Sin(this.angle * 0.017453292f);
		Vector2 linearVelocity;
		linearVelocity.x = x;
		linearVelocity.y = y;
		this.rb.linearVelocity = linearVelocity;
	}

	// Token: 0x0400153D RID: 5437
	public float speedMin;

	// Token: 0x0400153E RID: 5438
	public float speedMax;

	// Token: 0x0400153F RID: 5439
	public float turnAccelMax;

	// Token: 0x04001540 RID: 5440
	public float turnMax;

	// Token: 0x04001541 RID: 5441
	public float trailStartPause;

	// Token: 0x04001542 RID: 5442
	private Rigidbody2D rb;

	// Token: 0x04001543 RID: 5443
	private AraTrail trailRenderer;

	// Token: 0x04001544 RID: 5444
	private float angle;

	// Token: 0x04001545 RID: 5445
	private float speed;

	// Token: 0x04001546 RID: 5446
	private float turnSpeed;

	// Token: 0x04001547 RID: 5447
	private float turnAccel;

	// Token: 0x04001548 RID: 5448
	private float timer;

	// Token: 0x04001549 RID: 5449
	private Material[] materials;
}
