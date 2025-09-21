using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class ForceRamp2D : RampBase
{
	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600005F RID: 95 RVA: 0x00003F08 File Offset: 0x00002108
	// (set) Token: 0x06000060 RID: 96 RVA: 0x00003F10 File Offset: 0x00002110
	private Vector2 Force
	{
		get
		{
			return this.force;
		}
		set
		{
			this.force = value;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000061 RID: 97 RVA: 0x00003F19 File Offset: 0x00002119
	// (set) Token: 0x06000062 RID: 98 RVA: 0x00003F21 File Offset: 0x00002121
	public float Torque
	{
		get
		{
			return this.torque;
		}
		set
		{
			this.torque = value;
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00003F2A File Offset: 0x0000212A
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.hasBody = (this.body != null);
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003F4C File Offset: 0x0000214C
	private void FixedUpdate()
	{
		if (this.requireStarted && !this.started)
		{
			return;
		}
		if (!this.hasBody)
		{
			return;
		}
		if (this.addForce.sqrMagnitude > Mathf.Epsilon)
		{
			this.body.AddForce(this.addForce, ForceMode2D.Force);
		}
		if (Mathf.Abs(this.addTorque) > Mathf.Epsilon)
		{
			this.body.AddTorque(this.addTorque, ForceMode2D.Force);
		}
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003FBB File Offset: 0x000021BB
	protected override void UpdateValues(float multiplier)
	{
		this.addForce = this.force * multiplier;
		this.addTorque = this.torque * multiplier;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003FDD File Offset: 0x000021DD
	protected override void ResetValues()
	{
		this.addForce = Vector2.zero;
		this.addTorque = 0f;
	}

	// Token: 0x04000051 RID: 81
	[Space]
	[SerializeField]
	private Vector2 force;

	// Token: 0x04000052 RID: 82
	[SerializeField]
	private float torque;

	// Token: 0x04000053 RID: 83
	[Space]
	[SerializeField]
	private bool requireStarted;

	// Token: 0x04000054 RID: 84
	private Vector2 addForce;

	// Token: 0x04000055 RID: 85
	private float addTorque;

	// Token: 0x04000056 RID: 86
	private Rigidbody2D body;

	// Token: 0x04000057 RID: 87
	private bool hasBody;
}
