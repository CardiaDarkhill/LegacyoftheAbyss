using System;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class EnemyEdgeControl : MonoBehaviour
{
	// Token: 0x060019D0 RID: 6608 RVA: 0x0007690C File Offset: 0x00074B0C
	private void Start()
	{
		this.selfTransform = base.transform;
		Vector3 position = this.selfTransform.position;
		this.leftX = (this.edgeL ? this.edgeL.position.x : 0f);
		this.rightX = (this.edgeR ? this.edgeR.position.x : 9999f);
		this.rb = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x00076994 File Offset: 0x00074B94
	private void Update()
	{
		if (!this.stopped)
		{
			float x = this.selfTransform.position.x;
			if ((x < this.leftX - 1f && this.rb.linearVelocity.x < 0f) || (x > this.rightX + 1f && this.rb.linearVelocity.x > 0f))
			{
				this.Stop();
			}
		}
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x00076A0C File Offset: 0x00074C0C
	private void FixedUpdate()
	{
		if (!this.stopped)
		{
			float x = this.selfTransform.position.x;
			if (x < this.leftX)
			{
				if (this.rb.linearVelocity.x < 0f)
				{
					this.Decelerate();
				}
				if (!this.sentMsg)
				{
					if (this.notifyFSM)
					{
						this.notifyFSM.SendEvent("EDGE");
					}
					if (this.notifyFSM)
					{
						this.notifyFSM.SendEvent("EDGE L");
					}
					this.sentMsg = true;
					return;
				}
			}
			else if (x > this.rightX)
			{
				if (this.rb.linearVelocity.x > 0f)
				{
					this.Decelerate();
				}
				if (!this.sentMsg)
				{
					if (this.notifyFSM)
					{
						this.notifyFSM.SendEvent("EDGE");
					}
					if (this.notifyFSM)
					{
						this.notifyFSM.SendEvent("EDGE R");
					}
					this.sentMsg = true;
					return;
				}
			}
			else if (this.sentMsg)
			{
				this.sentMsg = false;
			}
		}
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x00076B28 File Offset: 0x00074D28
	private void Decelerate()
	{
		Vector2 linearVelocity = this.rb.linearVelocity;
		if (linearVelocity.x < 0f)
		{
			linearVelocity.x *= 0.85f;
			if (linearVelocity.x > 0f)
			{
				linearVelocity.x = 0f;
			}
		}
		else if (linearVelocity.x > 0f)
		{
			linearVelocity.x *= 0.85f;
			if (linearVelocity.x < 0f)
			{
				linearVelocity.x = 0f;
			}
		}
		if (linearVelocity.x < 0.001f && linearVelocity.x > -0.001f)
		{
			linearVelocity.x = 0f;
		}
		this.rb.linearVelocity = linearVelocity;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x00076BE1 File Offset: 0x00074DE1
	private void Stop()
	{
		this.rb.linearVelocity = new Vector2(0f, this.rb.linearVelocity.y);
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x00076C08 File Offset: 0x00074E08
	public void StopEdgeControl()
	{
		this.stopped = true;
	}

	// Token: 0x040018BF RID: 6335
	public Transform edgeL;

	// Token: 0x040018C0 RID: 6336
	public Transform edgeR;

	// Token: 0x040018C1 RID: 6337
	public PlayMakerFSM notifyFSM;

	// Token: 0x040018C2 RID: 6338
	private Transform selfTransform;

	// Token: 0x040018C3 RID: 6339
	private Rigidbody2D rb;

	// Token: 0x040018C4 RID: 6340
	private float leftX;

	// Token: 0x040018C5 RID: 6341
	private float rightX;

	// Token: 0x040018C6 RID: 6342
	private bool sentMsg;

	// Token: 0x040018C7 RID: 6343
	private bool stopped;

	// Token: 0x040018C8 RID: 6344
	private const float decelerationRate = 0.85f;
}
