using System;
using UnityEngine;

// Token: 0x020005D2 RID: 1490
internal class ProfilerRecording
{
	// Token: 0x060034ED RID: 13549 RVA: 0x000EAFB2 File Offset: 0x000E91B2
	public ProfilerRecording(string id)
	{
		this.id = id;
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x000EAFC1 File Offset: 0x000E91C1
	public void Start()
	{
		if (this.started)
		{
			this.BalanceError();
		}
		this.count++;
		this.started = true;
		this.startTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060034EF RID: 13551 RVA: 0x000EAFF4 File Offset: 0x000E91F4
	public void Stop()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!this.started)
		{
			this.BalanceError();
		}
		this.started = false;
		float num = realtimeSinceStartup - this.startTime;
		this.accumulatedTime += num;
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x000EB031 File Offset: 0x000E9231
	public void Reset()
	{
		this.accumulatedTime = 0f;
		this.count = 0;
		this.started = false;
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x000EB04C File Offset: 0x000E924C
	private void BalanceError()
	{
		Debug.LogError("ProfilerRecording start/stops not balanced for '" + this.id + "'");
	}

	// Token: 0x170005D8 RID: 1496
	// (get) Token: 0x060034F2 RID: 13554 RVA: 0x000EB068 File Offset: 0x000E9268
	public float Seconds
	{
		get
		{
			return this.accumulatedTime;
		}
	}

	// Token: 0x170005D9 RID: 1497
	// (get) Token: 0x060034F3 RID: 13555 RVA: 0x000EB070 File Offset: 0x000E9270
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x0400385E RID: 14430
	private int count;

	// Token: 0x0400385F RID: 14431
	private float startTime;

	// Token: 0x04003860 RID: 14432
	private float accumulatedTime;

	// Token: 0x04003861 RID: 14433
	private bool started;

	// Token: 0x04003862 RID: 14434
	public string id;
}
