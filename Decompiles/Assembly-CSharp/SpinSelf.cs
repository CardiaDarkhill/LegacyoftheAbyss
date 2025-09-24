using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005C9 RID: 1481
public class SpinSelf : MonoBehaviour
{
	// Token: 0x170005D6 RID: 1494
	// (get) Token: 0x060034CC RID: 13516 RVA: 0x000EA822 File Offset: 0x000E8A22
	// (set) Token: 0x060034CD RID: 13517 RVA: 0x000EA82A File Offset: 0x000E8A2A
	public float SpinFactor
	{
		get
		{
			return this.spinFactor;
		}
		set
		{
			this.spinFactor = value;
		}
	}

	// Token: 0x170005D7 RID: 1495
	// (get) Token: 0x060034CE RID: 13518 RVA: 0x000EA833 File Offset: 0x000E8A33
	// (set) Token: 0x060034CF RID: 13519 RVA: 0x000EA83B File Offset: 0x000E8A3B
	public bool RandomiseOnEnable
	{
		get
		{
			return this.randomiseOnEnable;
		}
		set
		{
			this.randomiseOnEnable = value;
		}
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x000EA844 File Offset: 0x000E8A44
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060034D1 RID: 13521 RVA: 0x000EA854 File Offset: 0x000E8A54
	private void OnEnable()
	{
		ComponentSingleton<SpinSelfCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
		if (this.RandomiseOnEnable)
		{
			Transform transform = base.transform;
			Vector3 localEulerAngles = transform.localEulerAngles;
			Vector3 original = localEulerAngles;
			float? z = new float?((float)Random.Range(0, 360));
			transform.localEulerAngles = original.Where(null, null, z);
		}
		this.spun = false;
		if (!this.body)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x000EA8D8 File Offset: 0x000E8AD8
	private void OnDisable()
	{
		ComponentSingleton<SpinSelfCallbackHooks>.Instance.OnFixedUpdate -= this.OnFixedUpdate;
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x000EA8F0 File Offset: 0x000E8AF0
	private void OnFixedUpdate()
	{
		if (this.spun)
		{
			return;
		}
		if (this.stepCounter >= 1)
		{
			float torque = this.body.linearVelocity.x * this.SpinFactor;
			this.body.AddTorque(torque);
			this.spun = true;
		}
		this.stepCounter++;
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x000EA948 File Offset: 0x000E8B48
	public void ReSpin()
	{
		this.spun = false;
		this.stepCounter = 0;
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x000EA958 File Offset: 0x000E8B58
	public void StopBounce()
	{
		this.spun = true;
		this.body.angularVelocity = 0f;
	}

	// Token: 0x04003849 RID: 14409
	[SerializeField]
	private float spinFactor = -7.5f;

	// Token: 0x0400384A RID: 14410
	[SerializeField]
	private bool randomiseOnEnable = true;

	// Token: 0x0400384B RID: 14411
	private int stepCounter;

	// Token: 0x0400384C RID: 14412
	private bool spun;

	// Token: 0x0400384D RID: 14413
	private Rigidbody2D body;
}
