using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class DropCrystal : MonoBehaviour
{
	// Token: 0x060013A2 RID: 5026 RVA: 0x00059720 File Offset: 0x00057920
	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, Random.Range(-0.01f, 0.01f));
		float num = Random.Range(0.4f, 1f);
		base.transform.localScale = new Vector3(num, num, num);
		this.startPos = base.transform.position;
		this.rb = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x000597AC File Offset: 0x000579AC
	public void OnEnable()
	{
		this.onConveyor = false;
		base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x000597E0 File Offset: 0x000579E0
	private void FixedUpdate()
	{
		if (this.stepCounter >= 10)
		{
			Vector2 a = new Vector2(base.transform.position.x, base.transform.position.y);
			this.velocity = a - this.lastPos;
			this.lastPos = a;
			this.speed = this.rb.linearVelocity.magnitude;
			if (base.transform.position.y < 4f)
			{
				base.transform.position = this.startPos;
				this.rb.linearVelocity = new Vector2(0f, 0f);
			}
			this.stepCounter = 0;
			return;
		}
		this.stepCounter++;
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x000598AC File Offset: 0x00057AAC
	private void OnCollisionEnter2D(Collision2D col)
	{
		if (this.speed > this.speedThreshold)
		{
			Vector3 inNormal = col.GetSafeContact().Normal;
			Vector3 normalized = Vector3.Reflect(this.velocity.normalized, inNormal).normalized;
			this.rb.linearVelocity = new Vector2(normalized.x, normalized.y) * (this.speed * (this.bounceFactor * Random.Range(0.8f, 1.2f)));
		}
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x00059938 File Offset: 0x00057B38
	private void LateUpdate()
	{
		if (this.onConveyor && this.xSpeed != 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x + this.xSpeed * Time.deltaTime, base.transform.position.y, base.transform.position.z);
		}
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x000599A7 File Offset: 0x00057BA7
	public void StartConveyorMove(float c_xSpeed, float c_ySpeed)
	{
		this.onConveyor = true;
		this.xSpeed = c_xSpeed;
		this.ySpeed = c_ySpeed;
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x000599BE File Offset: 0x00057BBE
	public void StopConveyorMove()
	{
		this.onConveyor = false;
	}

	// Token: 0x0400120A RID: 4618
	public float bounceFactor;

	// Token: 0x0400120B RID: 4619
	public float speedThreshold = 1f;

	// Token: 0x0400120C RID: 4620
	private float speed;

	// Token: 0x0400120D RID: 4621
	private float animTimer;

	// Token: 0x0400120E RID: 4622
	private Vector2 velocity;

	// Token: 0x0400120F RID: 4623
	private Vector2 lastPos;

	// Token: 0x04001210 RID: 4624
	private Rigidbody2D rb;

	// Token: 0x04001211 RID: 4625
	private int chooser;

	// Token: 0x04001212 RID: 4626
	private int stepCounter;

	// Token: 0x04001213 RID: 4627
	private float xSpeed;

	// Token: 0x04001214 RID: 4628
	private float ySpeed;

	// Token: 0x04001215 RID: 4629
	private bool onConveyor;

	// Token: 0x04001216 RID: 4630
	private Vector3 startPos;
}
