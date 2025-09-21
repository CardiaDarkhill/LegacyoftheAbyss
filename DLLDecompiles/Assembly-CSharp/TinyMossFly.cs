using System;
using UnityEngine;

// Token: 0x02000576 RID: 1398
[RequireComponent(typeof(Rigidbody2D))]
public class TinyMossFly : MonoBehaviour
{
	// Token: 0x06003212 RID: 12818 RVA: 0x000DEF4C File Offset: 0x000DD14C
	protected void Reset()
	{
		this.waitMin = 0.75f;
		this.waitMax = 1f;
		this.speedMax = 1.75f;
		this.accelerationMax = 15f;
		this.roamingRange = 1f;
		this.dampener = 1.125f;
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x000DEF9B File Offset: 0x000DD19B
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x000DEFA9 File Offset: 0x000DD1A9
	protected void Start()
	{
		this.start2D = this.body.position;
		this.acceleration2D = Vector2.zero;
		this.Buzz(0f);
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x000DEFD4 File Offset: 0x000DD1D4
	protected void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (!this.flyingAway && !this.songMode)
		{
			this.Buzz(deltaTime);
		}
		if (this.songMode)
		{
			Vector3 position = new Vector3(this.startX + Random.Range(-0.06f, 0.06f), this.startY + Random.Range(-0.06f, 0.06f), base.transform.position.z);
			base.transform.position = position;
		}
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x000DF058 File Offset: 0x000DD258
	protected void Update()
	{
		if (this.faceDirection)
		{
			if (this.body.linearVelocity.x < 0f && base.transform.localScale.x > 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			}
			else if (this.body.linearVelocity.x > 0f && base.transform.localScale.x < 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			}
		}
		if (this.flyingAway)
		{
			this.body.linearVelocity = new Vector2(this.flyAwaySpeed, this.flyAwaySpeedY);
			this.flyAwaySpeed += this.accelerator * Time.deltaTime;
			if (this.endTimer > 0f)
			{
				this.endTimer -= Time.deltaTime;
				return;
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x000DF1C4 File Offset: 0x000DD3C4
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

	// Token: 0x06003218 RID: 12824 RVA: 0x000DF348 File Offset: 0x000DD548
	public void FlyAway()
	{
		this.flyAwaySpeed = Random.Range(3f, 5f) * base.transform.localScale.x;
		this.flyAwaySpeedY = Random.Range(1f, 5f);
		this.accelerator = Random.Range(8f, 11f) * base.transform.localScale.x;
		this.flyingAway = true;
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x000DF3C0 File Offset: 0x000DD5C0
	public void SongStart()
	{
		this.startX = base.transform.position.x;
		this.startY = base.transform.position.y;
		if (base.transform.localScale.x > 0f)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(12f, 14f));
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-12f, -14f));
		}
		this.songMode = true;
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x000DF46B File Offset: 0x000DD66B
	public void SongStop()
	{
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.songMode = false;
	}

	// Token: 0x040035A9 RID: 13737
	private Rigidbody2D body;

	// Token: 0x040035AA RID: 13738
	[SerializeField]
	private float waitMin;

	// Token: 0x040035AB RID: 13739
	[SerializeField]
	private float waitMax;

	// Token: 0x040035AC RID: 13740
	[SerializeField]
	private float speedMax;

	// Token: 0x040035AD RID: 13741
	[SerializeField]
	private float accelerationMax;

	// Token: 0x040035AE RID: 13742
	[SerializeField]
	private float roamingRange;

	// Token: 0x040035AF RID: 13743
	[SerializeField]
	private float dampener;

	// Token: 0x040035B0 RID: 13744
	[SerializeField]
	private bool faceDirection;

	// Token: 0x040035B1 RID: 13745
	private Vector2 start2D;

	// Token: 0x040035B2 RID: 13746
	private Vector2 acceleration2D;

	// Token: 0x040035B3 RID: 13747
	private float waitTimer;

	// Token: 0x040035B4 RID: 13748
	private bool flyingAway;

	// Token: 0x040035B5 RID: 13749
	private bool songMode;

	// Token: 0x040035B6 RID: 13750
	private float flyAwaySpeed;

	// Token: 0x040035B7 RID: 13751
	private float flyAwaySpeedY;

	// Token: 0x040035B8 RID: 13752
	private float accelerator;

	// Token: 0x040035B9 RID: 13753
	private float endTimer = 3f;

	// Token: 0x040035BA RID: 13754
	private float startX;

	// Token: 0x040035BB RID: 13755
	private float startY;

	// Token: 0x040035BC RID: 13756
	private const float InspectorAccelerationConstant = 2000f;
}
