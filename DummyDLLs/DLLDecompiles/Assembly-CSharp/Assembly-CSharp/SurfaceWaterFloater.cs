using System;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000564 RID: 1380
public class SurfaceWaterFloater : MonoBehaviour
{
	// Token: 0x17000557 RID: 1367
	// (get) Token: 0x06003141 RID: 12609 RVA: 0x000DAA50 File Offset: 0x000D8C50
	public float FloatHeight
	{
		get
		{
			return this.floatHeight;
		}
	}

	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x06003142 RID: 12610 RVA: 0x000DAA58 File Offset: 0x000D8C58
	public float GravityScale
	{
		get
		{
			return this.body.gravityScale;
		}
	}

	// Token: 0x17000559 RID: 1369
	// (get) Token: 0x06003143 RID: 12611 RVA: 0x000DAA65 File Offset: 0x000D8C65
	public float Velocity
	{
		get
		{
			return this.body.linearVelocity.y;
		}
	}

	// Token: 0x1700055A RID: 1370
	// (get) Token: 0x06003144 RID: 12612 RVA: 0x000DAA78 File Offset: 0x000D8C78
	public float FloatMultiplier
	{
		get
		{
			if (this.shouldSink)
			{
				return 0f;
			}
			if (this.dropTimer <= 0f || Time.timeAsDouble < this.dropTime)
			{
				return 1f;
			}
			if (this.waitingForDrop)
			{
				this.waitingForDrop = false;
				if (this.objectBounce)
				{
					this.objectBounce.StopBounce();
				}
			}
			float num = (float)(Time.timeAsDouble - this.dropTime);
			float num2 = Mathf.Clamp01(num / this.dropDuration);
			if (num - this.dropDuration > this.despawnDelay && this.canRecycle)
			{
				this.canRecycle = false;
				base.gameObject.Recycle();
			}
			return 1f - num2;
		}
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x000DAB28 File Offset: 0x000D8D28
	private void Awake()
	{
		if (this.eventTarget)
		{
			this.inWaterFsmBool = this.eventTarget.FsmVariables.FindFsmBool("Is Floating");
		}
		this.feather = base.GetComponent<FeatherPhysics>();
		this.objectBounce = base.GetComponent<ObjectBounce>();
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x000DAB75 File Offset: 0x000D8D75
	private void OnEnable()
	{
		this.canRecycle = true;
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x000DAB7E File Offset: 0x000D8D7E
	private void OnDisable()
	{
		if (this.feather)
		{
			this.feather.enabled = true;
		}
		this.shouldSink = false;
		this.canRecycle = false;
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x000DABA8 File Offset: 0x000D8DA8
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		position.y += this.floatHeight;
		Gizmos.DrawWireSphere(position, 0.1f);
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x000DABDD File Offset: 0x000D8DDD
	public void AddForce(float force)
	{
		force *= this.body.mass;
		this.body.AddForce(new Vector2(0f, force));
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x000DAC04 File Offset: 0x000D8E04
	public void AddFlowSpeed(float flowSpeed, Quaternion surfaceRotation)
	{
		if (Math.Abs(flowSpeed) <= Mathf.Epsilon)
		{
			return;
		}
		Vector2 linearVelocity = this.body.linearVelocity;
		linearVelocity.x = flowSpeed;
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x000DAC40 File Offset: 0x000D8E40
	public void MoveWithSurface(float flowSpeed, Quaternion surfaceRotation)
	{
		if (Math.Abs(flowSpeed) <= Mathf.Epsilon)
		{
			return;
		}
		Vector3 v = surfaceRotation * new Vector2(flowSpeed, 0f);
		v.y += this.body.gravityScale * Physics2D.gravity.y * -1f * Time.fixedDeltaTime;
		this.body.linearVelocity = v;
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x000DACB0 File Offset: 0x000D8EB0
	public void SetInWater(bool value)
	{
		if (this.inWaterFsmBool != null)
		{
			this.inWaterFsmBool.Value = value;
		}
		if (value)
		{
			this.dropTime = Time.timeAsDouble + (double)this.dropTimer;
			this.waitingForDrop = true;
			this.splashInAudio.SpawnAndPlayOneShot(base.transform.position, false);
			this.OnLandInWater.Invoke();
			if (this.eventTarget)
			{
				this.eventTarget.SendEvent("ENTERED FLOAT");
				return;
			}
		}
		else
		{
			this.OnExitWater.Invoke();
			if (this.eventTarget)
			{
				this.eventTarget.SendEvent("EXITED FLOAT");
			}
		}
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x000DAD58 File Offset: 0x000D8F58
	public void Dampen()
	{
		Vector2 linearVelocity = this.body.linearVelocity;
		linearVelocity.x *= this.waterDampX;
		this.body.linearVelocity = linearVelocity;
		float num = this.body.angularVelocity;
		num *= this.waterDampAngular;
		this.body.angularVelocity = num;
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x000DADAF File Offset: 0x000D8FAF
	public void Sink()
	{
		this.shouldSink = true;
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x000DADB8 File Offset: 0x000D8FB8
	public void StopSink()
	{
		this.shouldSink = false;
	}

	// Token: 0x0400349D RID: 13469
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x0400349E RID: 13470
	[SerializeField]
	private float floatHeight;

	// Token: 0x0400349F RID: 13471
	[SerializeField]
	private float dropTimer;

	// Token: 0x040034A0 RID: 13472
	[SerializeField]
	private float dropDuration;

	// Token: 0x040034A1 RID: 13473
	[SerializeField]
	private float despawnDelay = 5f;

	// Token: 0x040034A2 RID: 13474
	[SerializeField]
	private float waterDampX = 0.9f;

	// Token: 0x040034A3 RID: 13475
	[SerializeField]
	private float waterDampAngular = 0.9f;

	// Token: 0x040034A4 RID: 13476
	[SerializeField]
	private RandomAudioClipTable splashInAudio;

	// Token: 0x040034A5 RID: 13477
	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	// Token: 0x040034A6 RID: 13478
	[Space]
	public UnityEvent OnLandInWater;

	// Token: 0x040034A7 RID: 13479
	public UnityEvent OnExitWater;

	// Token: 0x040034A8 RID: 13480
	private double dropTime;

	// Token: 0x040034A9 RID: 13481
	private FsmBool inWaterFsmBool;

	// Token: 0x040034AA RID: 13482
	private bool shouldSink;

	// Token: 0x040034AB RID: 13483
	private FeatherPhysics feather;

	// Token: 0x040034AC RID: 13484
	private ObjectBounce objectBounce;

	// Token: 0x040034AD RID: 13485
	private bool waitingForDrop;

	// Token: 0x040034AE RID: 13486
	private bool canRecycle;
}
