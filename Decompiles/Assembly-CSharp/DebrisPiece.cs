using System;
using UnityEngine;

// Token: 0x020004CB RID: 1227
[RequireComponent(typeof(Rigidbody2D))]
public class DebrisPiece : MonoBehaviour
{
	// Token: 0x06002C2D RID: 11309 RVA: 0x000C185E File Offset: 0x000BFA5E
	protected void Reset()
	{
		this.resetOnDisable = true;
		this.forceZ = true;
		this.forcedZ = 0.015f;
		this.randomStartRotation = false;
		this.zRandomRadius = 0.000999f;
		this.spinFactor = 10f;
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x000C1896 File Offset: 0x000BFA96
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		if (this.body == null)
		{
			Debug.LogErrorFormat(this, "Missing Rigidbody2D on {0}", new object[]
			{
				base.name
			});
		}
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x000C18CC File Offset: 0x000BFACC
	protected void OnEnable()
	{
		if (!this.didLaunch)
		{
			this.Launch();
		}
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x000C18DC File Offset: 0x000BFADC
	protected void OnDisable()
	{
		if (this.resetOnDisable)
		{
			this.didLaunch = false;
			this.didSpin = false;
		}
	}

	// Token: 0x06002C31 RID: 11313 RVA: 0x000C18F4 File Offset: 0x000BFAF4
	private void Launch()
	{
		this.didLaunch = true;
		Transform transform = base.transform;
		if (this.forceZ)
		{
			Vector3 position = transform.position;
			position.z = this.forcedZ;
			transform.position = position;
		}
		if (this.randomStartRotation)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.z = Random.Range(0f, 360f);
			transform.localEulerAngles = localEulerAngles;
		}
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x000C195E File Offset: 0x000BFB5E
	protected void FixedUpdate()
	{
		if (!this.didSpin)
		{
			this.Spin();
		}
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x000C1970 File Offset: 0x000BFB70
	private void Spin()
	{
		this.didSpin = true;
		if (Math.Abs(this.spinFactor) <= Mathf.Epsilon)
		{
			return;
		}
		if (Math.Abs(this.zRandomRadius) > Mathf.Epsilon)
		{
			Vector3 position = base.transform.position;
			position.z += Random.Range(-this.zRandomRadius, this.zRandomRadius);
			base.transform.position = position;
		}
		this.body.AddTorque(-this.body.linearVelocity.x * this.spinFactor);
	}

	// Token: 0x04002D97 RID: 11671
	[SerializeField]
	private bool resetOnDisable;

	// Token: 0x04002D98 RID: 11672
	private bool didLaunch;

	// Token: 0x04002D99 RID: 11673
	private bool didSpin;

	// Token: 0x04002D9A RID: 11674
	[Header("'set_z' Functionality")]
	[SerializeField]
	private bool forceZ;

	// Token: 0x04002D9B RID: 11675
	[SerializeField]
	[ModifiableProperty]
	[Conditional("forceZ", true, false, false)]
	private float forcedZ;

	// Token: 0x04002D9C RID: 11676
	[Header("'spin_self' Functionality")]
	[SerializeField]
	private bool randomStartRotation;

	// Token: 0x04002D9D RID: 11677
	[SerializeField]
	private float zRandomRadius;

	// Token: 0x04002D9E RID: 11678
	[SerializeField]
	private float spinFactor;

	// Token: 0x04002D9F RID: 11679
	private Rigidbody2D body;
}
