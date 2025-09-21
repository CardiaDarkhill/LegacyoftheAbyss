using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class FlingSelf : MonoBehaviour
{
	// Token: 0x060014A4 RID: 5284 RVA: 0x0005CFEA File Offset: 0x0005B1EA
	private void Start()
	{
		if (!this.onEnable)
		{
			this.DoFling();
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0005CFFA File Offset: 0x0005B1FA
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.DoFling();
		}
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0005D00C File Offset: 0x0005B20C
	public void DoFling()
	{
		Rigidbody2D component = base.gameObject.GetComponent<Rigidbody2D>();
		if (component != null)
		{
			Transform transform = base.transform;
			float num = Random.Range(this.speedMin, this.speedMax);
			float num2 = Random.Range(this.angleMin, this.angleMax);
			float num3 = num * Mathf.Cos(num2 * 0.017453292f);
			float num4 = num * Mathf.Sin(num2 * 0.017453292f);
			if (this.relativeToParent)
			{
				Vector3 lossyScale = transform.lossyScale;
				num3 *= Mathf.Sign(lossyScale.x);
				num4 *= Mathf.Sign(lossyScale.y);
			}
			if (this.deparent)
			{
				transform.parent = null;
			}
			component.linearVelocity = new Vector2(num3, num4);
		}
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x0005D0C6 File Offset: 0x0005B2C6
	public void SetSpeedMin(float newSpeed)
	{
		this.speedMin = newSpeed;
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x0005D0CF File Offset: 0x0005B2CF
	public void SetSpeedMax(float newSpeed)
	{
		this.speedMax = newSpeed;
	}

	// Token: 0x04001302 RID: 4866
	public float speedMin;

	// Token: 0x04001303 RID: 4867
	public float speedMax;

	// Token: 0x04001304 RID: 4868
	public float angleMin;

	// Token: 0x04001305 RID: 4869
	public float angleMax;

	// Token: 0x04001306 RID: 4870
	public bool relativeToParent;

	// Token: 0x04001307 RID: 4871
	public bool deparent = true;

	// Token: 0x04001308 RID: 4872
	public bool onEnable;
}
