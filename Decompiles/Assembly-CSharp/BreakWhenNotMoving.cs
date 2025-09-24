using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004AD RID: 1197
public class BreakWhenNotMoving : MonoBehaviour
{
	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x06002B4E RID: 11086 RVA: 0x000BDF08 File Offset: 0x000BC108
	public bool IsBroken
	{
		get
		{
			return this.isBroken;
		}
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000BDF10 File Offset: 0x000BC110
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000BDF20 File Offset: 0x000BC120
	private void OnEnable()
	{
		this.isBroken = false;
		Renderer[] array = this.activeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		ParticleSystem[] array2 = this.breakParticles;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop(true);
		}
		this.breakEffects.SetAllActive(false);
		if (this.body)
		{
			this.body.simulated = true;
		}
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000BDF98 File Offset: 0x000BC198
	private void Update()
	{
		if (this.isBroken)
		{
			bool flag = false;
			ParticleSystem[] array = this.breakParticles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(true))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				base.gameObject.Recycle();
				return;
			}
		}
		else
		{
			Vector3 position = base.transform.position;
			Vector2 a = position - this.previousPosition;
			this.previousPosition = position;
			if ((a / Time.deltaTime).magnitude > this.movementThreshold)
			{
				this.breakTime = Time.timeAsDouble + (double)this.waitTime;
			}
			if (Time.timeAsDouble > this.breakTime)
			{
				this.Break();
			}
		}
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000BE04C File Offset: 0x000BC24C
	public void Break()
	{
		this.isBroken = true;
		Renderer[] array = this.activeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		ParticleSystem[] array2 = this.breakParticles;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Play(true);
		}
		this.breakEffects.SetAllActive(true);
		if (this.body)
		{
			this.body.simulated = false;
		}
		this.OnBreak.Invoke();
	}

	// Token: 0x04002C98 RID: 11416
	[SerializeField]
	private Renderer[] activeRenderers;

	// Token: 0x04002C99 RID: 11417
	[SerializeField]
	private ParticleSystem[] breakParticles;

	// Token: 0x04002C9A RID: 11418
	[SerializeField]
	private GameObject[] breakEffects;

	// Token: 0x04002C9B RID: 11419
	[SerializeField]
	private float movementThreshold;

	// Token: 0x04002C9C RID: 11420
	[SerializeField]
	private float waitTime;

	// Token: 0x04002C9D RID: 11421
	public UnityEvent OnBreak;

	// Token: 0x04002C9E RID: 11422
	private double breakTime;

	// Token: 0x04002C9F RID: 11423
	private Vector2 previousPosition;

	// Token: 0x04002CA0 RID: 11424
	private bool isBroken;

	// Token: 0x04002CA1 RID: 11425
	private Rigidbody2D body;
}
