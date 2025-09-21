using System;
using UnityEngine;

// Token: 0x02000507 RID: 1287
[RequireComponent(typeof(Rigidbody2D))]
public class IdleBuzzing : MonoBehaviour
{
	// Token: 0x06002E03 RID: 11779 RVA: 0x000C9BDC File Offset: 0x000C7DDC
	protected void Reset()
	{
		this.waitMin = 0.75f;
		this.waitMax = 1f;
		this.speedMax = 1.75f;
		this.accelerationMax = 15f;
		this.roamingRange = 1f;
		this.dampener = 1.125f;
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x000C9C2B File Offset: 0x000C7E2B
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000C9C39 File Offset: 0x000C7E39
	protected void Start()
	{
		this.start2D = this.body.position;
		this.acceleration2D = Vector2.zero;
		this.Buzz(0f);
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000C9C64 File Offset: 0x000C7E64
	protected void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		this.Buzz(deltaTime);
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000C9C80 File Offset: 0x000C7E80
	protected void Update()
	{
		if (this.faceDirection)
		{
			if (this.body.linearVelocity.x < 0f && base.transform.localScale.x > 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
				return;
			}
			if (this.body.linearVelocity.x > 0f && base.transform.localScale.x < 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			}
		}
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x000C9D84 File Offset: 0x000C7F84
	private void Buzz(float deltaTime)
	{
		Vector2 position = this.body.position;
		Vector2 linearVelocity = this.body.linearVelocity;
		bool flag;
		if (this.waitTimer <= 0f)
		{
			flag = true;
			this.waitTimer = Random.Range(this.waitMin, this.waitMax);
		}
		else
		{
			this.waitTimer -= deltaTime;
			flag = false;
		}
		for (int i = 0; i < 2; i++)
		{
			float num = linearVelocity[i];
			float num2 = this.start2D[i];
			float num3 = position[i] - num2;
			float num4 = this.acceleration2D[i];
			if (flag)
			{
				if (Mathf.Abs(num3) > this.roamingRange)
				{
					num4 = -Mathf.Sign(num3) * this.accelerationMax;
				}
				else
				{
					num4 = Random.Range(-this.accelerationMax, this.accelerationMax);
				}
				num4 /= 2000f;
			}
			else if (Mathf.Abs(num3) > this.roamingRange && num3 > 0f == num > 0f)
			{
				num4 = this.accelerationMax * -Mathf.Sign(num3) / 2000f;
				num /= this.dampener;
				this.waitTimer = Random.Range(this.waitMin, this.waitMax);
			}
			num += num4;
			num = Mathf.Clamp(num, -this.speedMax, this.speedMax);
			linearVelocity[i] = num;
			this.acceleration2D[i] = num4;
		}
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x0400302F RID: 12335
	private Rigidbody2D body;

	// Token: 0x04003030 RID: 12336
	[SerializeField]
	private float waitMin;

	// Token: 0x04003031 RID: 12337
	[SerializeField]
	private float waitMax;

	// Token: 0x04003032 RID: 12338
	[SerializeField]
	private float speedMax;

	// Token: 0x04003033 RID: 12339
	[SerializeField]
	private float accelerationMax;

	// Token: 0x04003034 RID: 12340
	[SerializeField]
	private float roamingRange;

	// Token: 0x04003035 RID: 12341
	[SerializeField]
	private float dampener;

	// Token: 0x04003036 RID: 12342
	[SerializeField]
	private bool faceDirection;

	// Token: 0x04003037 RID: 12343
	private Vector2 start2D;

	// Token: 0x04003038 RID: 12344
	private Vector2 acceleration2D;

	// Token: 0x04003039 RID: 12345
	private float waitTimer;

	// Token: 0x0400303A RID: 12346
	private const float InspectorAccelerationConstant = 2000f;
}
