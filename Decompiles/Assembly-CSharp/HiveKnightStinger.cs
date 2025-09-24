using System;
using UnityEngine;

// Token: 0x020002EE RID: 750
public class HiveKnightStinger : MonoBehaviour
{
	// Token: 0x06001AD8 RID: 6872 RVA: 0x0007D09C File Offset: 0x0007B29C
	private void OnEnable()
	{
		if (!this.initialised)
		{
			this.startPos = base.transform.localPosition;
			this.initialised = true;
		}
		else
		{
			base.transform.localPosition = this.startPos;
		}
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody2D>();
		}
		this.timer = this.time;
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x0007D104 File Offset: 0x0007B304
	private void Update()
	{
		float x = this.speed * Mathf.Cos(this.direction * 0.017453292f);
		float y = this.speed * Mathf.Sin(this.direction * 0.017453292f);
		Vector2 linearVelocity;
		linearVelocity.x = x;
		linearVelocity.y = y;
		this.rb.linearVelocity = linearVelocity;
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x040019ED RID: 6637
	public float direction;

	// Token: 0x040019EE RID: 6638
	private float speed = 20f;

	// Token: 0x040019EF RID: 6639
	private float time = 2f;

	// Token: 0x040019F0 RID: 6640
	private float timer;

	// Token: 0x040019F1 RID: 6641
	private bool initialised;

	// Token: 0x040019F2 RID: 6642
	private Rigidbody2D rb;

	// Token: 0x040019F3 RID: 6643
	private Vector3 startPos;
}
