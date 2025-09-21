using System;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class SpinSelfSimple : MonoBehaviour
{
	// Token: 0x06000543 RID: 1347 RVA: 0x0001AC3B File Offset: 0x00018E3B
	private void Update()
	{
		if (this.timing && !this.waitForCall)
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.timing = false;
			this.DoSpin();
		}
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001AC7C File Offset: 0x00018E7C
	private void OnEnable()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody2D>();
		}
		if (this.randomStartRotation)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
		}
		this.timing = true;
		this.timer = 0.01f;
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001ACE8 File Offset: 0x00018EE8
	public void DoSpin()
	{
		float torque = -(this.rb.linearVelocity.x * this.spinFactor);
		this.rb.AddTorque(torque, ForceMode2D.Force);
	}

	// Token: 0x04000517 RID: 1303
	public bool randomStartRotation;

	// Token: 0x04000518 RID: 1304
	public float spinFactor;

	// Token: 0x04000519 RID: 1305
	public bool waitForCall;

	// Token: 0x0400051A RID: 1306
	public Rigidbody2D rb;

	// Token: 0x0400051B RID: 1307
	private bool timing;

	// Token: 0x0400051C RID: 1308
	private float timer;
}
