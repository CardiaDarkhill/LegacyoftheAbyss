using System;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class Dragonfly : MonoBehaviour
{
	// Token: 0x06002C49 RID: 11337 RVA: 0x000C1D9A File Offset: 0x000BFF9A
	private void Start()
	{
		this.initPos = base.transform.position;
		this.hero = GameManager.instance.hero_ctrl.gameObject;
		this.meshRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x000C1DD0 File Offset: 0x000BFFD0
	private void Update()
	{
		if (this.state == 0)
		{
			if (this.meshRenderer.isVisible)
			{
				this.StartIdle();
				return;
			}
		}
		else
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f && this.state == 1)
			{
				this.state = 2;
			}
		}
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x000C1E28 File Offset: 0x000C0028
	private void StartIdle()
	{
		this.rb.linearVelocity = new Vector3(0f, 0f, 0f);
		this.timer = Random.Range(this.idleMin, this.idleMax);
		this.state = 1;
	}

	// Token: 0x04002DBD RID: 11709
	public float xRange;

	// Token: 0x04002DBE RID: 11710
	public float yRange;

	// Token: 0x04002DBF RID: 11711
	public float idleMin = 0.5f;

	// Token: 0x04002DC0 RID: 11712
	public float idleMax = 1.5f;

	// Token: 0x04002DC1 RID: 11713
	public float flyMin = 0.15f;

	// Token: 0x04002DC2 RID: 11714
	public float flyMax = 0.15f;

	// Token: 0x04002DC3 RID: 11715
	private int state;

	// Token: 0x04002DC4 RID: 11716
	private float timer;

	// Token: 0x04002DC5 RID: 11717
	private GameObject hero;

	// Token: 0x04002DC6 RID: 11718
	private MeshRenderer meshRenderer;

	// Token: 0x04002DC7 RID: 11719
	private Rigidbody2D rb;

	// Token: 0x04002DC8 RID: 11720
	private Vector3 initPos;
}
