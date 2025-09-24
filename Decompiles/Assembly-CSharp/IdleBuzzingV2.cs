using System;
using UnityEngine;

// Token: 0x02000508 RID: 1288
[RequireComponent(typeof(Rigidbody2D))]
public class IdleBuzzingV2 : MonoBehaviour
{
	// Token: 0x06002E0A RID: 11786 RVA: 0x000C9F10 File Offset: 0x000C8110
	protected void Reset()
	{
		this.waitMin = 0.75f;
		this.waitMax = 1f;
		this.speedMax = 1.75f;
		this.accelerationMax = 15f;
		this.roamingRange = 1f;
		this.dampener = 1.125f;
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x000C9F5F File Offset: 0x000C815F
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000C9F6D File Offset: 0x000C816D
	protected void Start()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.start2D = this.body.position;
		this.acceleration2D = Vector2.zero;
		this.Buzz(0f);
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000C9FA4 File Offset: 0x000C81A4
	protected void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		this.Buzz(deltaTime);
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000C9FC0 File Offset: 0x000C81C0
	protected void Update()
	{
		if (this.faceDirection)
		{
			if (this.body.linearVelocity.x < 0f && base.transform.localScale.x > 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
				if (this.animator)
				{
					this.animator.Play(this.turnAnim);
					this.animator.PlayFromFrame(0);
					return;
				}
			}
			else if (this.body.linearVelocity.x > 0f && base.transform.localScale.x < 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
				if (this.animator)
				{
					this.animator.Play(this.turnAnim);
					this.animator.PlayFromFrame(0);
				}
			}
		}
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000CA120 File Offset: 0x000C8320
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

	// Token: 0x0400303B RID: 12347
	private Rigidbody2D body;

	// Token: 0x0400303C RID: 12348
	[SerializeField]
	private float waitMin;

	// Token: 0x0400303D RID: 12349
	[SerializeField]
	private float waitMax;

	// Token: 0x0400303E RID: 12350
	[SerializeField]
	private float speedMax;

	// Token: 0x0400303F RID: 12351
	[SerializeField]
	private float accelerationMax;

	// Token: 0x04003040 RID: 12352
	[SerializeField]
	private float roamingRange;

	// Token: 0x04003041 RID: 12353
	[SerializeField]
	private float dampener;

	// Token: 0x04003042 RID: 12354
	[SerializeField]
	private bool faceDirection;

	// Token: 0x04003043 RID: 12355
	[SerializeField]
	private string turnAnim;

	// Token: 0x04003044 RID: 12356
	private Vector2 start2D;

	// Token: 0x04003045 RID: 12357
	private Vector2 acceleration2D;

	// Token: 0x04003046 RID: 12358
	private float waitTimer;

	// Token: 0x04003047 RID: 12359
	private tk2dSpriteAnimator animator;

	// Token: 0x04003048 RID: 12360
	private const float InspectorAccelerationConstant = 2000f;
}
