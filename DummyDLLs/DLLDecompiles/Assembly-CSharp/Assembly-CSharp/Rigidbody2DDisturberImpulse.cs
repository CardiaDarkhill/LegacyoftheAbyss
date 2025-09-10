using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000540 RID: 1344
public class Rigidbody2DDisturberImpulse : Rigidbody2DDisturberBase
{
	// Token: 0x0600302C RID: 12332 RVA: 0x000D48B4 File Offset: 0x000D2AB4
	private void OnValidate()
	{
		if (this.directionRange != Vector2.zero)
		{
			this.minDirection = -this.directionRange;
			this.maxDirection = this.directionRange;
			this.directionRange = Vector2.zero;
		}
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x000D48F0 File Offset: 0x000D2AF0
	protected override void Awake()
	{
		this.OnValidate();
		base.Awake();
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x000D4900 File Offset: 0x000D2B00
	public void Impulse()
	{
		float num = (float)(Time.timeAsDouble - this.lastFullImpulseTime);
		float num2 = 0.01f;
		if (this.customCooldown)
		{
			num2 = this.cooldown;
		}
		float num4;
		if (this.limitByFrequency && num < num2)
		{
			float num3 = num / num2;
			if (this.useCoolDownCurve)
			{
				num3 = this.cooldownCurve.Evaluate(Mathf.Clamp01(num3));
			}
			else if (num3 < 0.5f)
			{
				num3 = 0.5f;
			}
			num4 = num3;
		}
		else
		{
			num4 = 1f;
			this.lastFullImpulseTime = Time.timeAsDouble;
		}
		foreach (Rigidbody2D rigidbody2D in this.bodies)
		{
			Vector2 a = new Vector2(Random.Range(this.minDirection.x, this.maxDirection.x), Random.Range(this.minDirection.y, this.maxDirection.y));
			a.Normalize();
			float num5 = num4;
			if (this.useDiminishingReturns)
			{
				if (this.debugMe)
				{
					this.debugMe = true;
				}
				float magnitude = rigidbody2D.linearVelocity.magnitude;
				float t = Mathf.Clamp01(this.diminishingRange.GetTBetween(magnitude));
				float lerpedValue = this.diminishingMultiplier.GetLerpedValue(t);
				if (this.debugMe)
				{
					this.debugMe = true;
				}
				num5 *= lerpedValue;
			}
			if (this.resetVelocity)
			{
				rigidbody2D.linearVelocity = Vector2.zero;
			}
			Vector2 vector = a * (this.magnitudeRange.GetRandomValue() * num5);
			if (this.debugMe)
			{
				Debug.Log(string.Format("Adding force: {0} to {1} {2} {3}", new object[]
				{
					vector,
					this,
					Time.frameCount,
					CustomPlayerLoop.FixedUpdateCycle
				}), this);
			}
			rigidbody2D.AddForce(vector, ForceMode2D.Impulse);
		}
	}

	// Token: 0x040032FD RID: 13053
	[SerializeField]
	[HideInInspector]
	private Vector2 directionRange = Vector2.one;

	// Token: 0x040032FE RID: 13054
	[SerializeField]
	private Vector2 minDirection;

	// Token: 0x040032FF RID: 13055
	[SerializeField]
	private Vector2 maxDirection;

	// Token: 0x04003300 RID: 13056
	[SerializeField]
	private MinMaxFloat magnitudeRange;

	// Token: 0x04003301 RID: 13057
	[SerializeField]
	private bool resetVelocity;

	// Token: 0x04003302 RID: 13058
	[SerializeField]
	private bool limitByFrequency;

	// Token: 0x04003303 RID: 13059
	[SerializeField]
	private bool customCooldown;

	// Token: 0x04003304 RID: 13060
	[SerializeField]
	private float cooldown = 0.5f;

	// Token: 0x04003305 RID: 13061
	[SerializeField]
	private bool useCoolDownCurve;

	// Token: 0x04003306 RID: 13062
	[SerializeField]
	private AnimationCurve cooldownCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04003307 RID: 13063
	[SerializeField]
	private bool useDiminishingReturns;

	// Token: 0x04003308 RID: 13064
	[SerializeField]
	private MinMaxFloat diminishingRange = new MinMaxFloat(45f, 65f);

	// Token: 0x04003309 RID: 13065
	[SerializeField]
	private MinMaxFloat diminishingMultiplier = new MinMaxFloat(1f, 0.05f);

	// Token: 0x0400330A RID: 13066
	[SerializeField]
	private bool debugMe;

	// Token: 0x0400330B RID: 13067
	private double lastFullImpulseTime;

	// Token: 0x0400330C RID: 13068
	private Coroutine rumbleRoutine;
}
