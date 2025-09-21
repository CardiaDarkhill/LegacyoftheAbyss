using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class MatchMasterTimeSample : MonoBehaviour
{
	// Token: 0x060008E2 RID: 2274 RVA: 0x00029A6C File Offset: 0x00027C6C
	private void Update()
	{
		this.slave1.timeSamples = this.master.timeSamples;
		this.slave2.timeSamples = this.master.timeSamples;
		this.slave3.timeSamples = this.master.timeSamples;
		this.slave4.timeSamples = this.master.timeSamples;
	}

	// Token: 0x04000898 RID: 2200
	public AudioSource master;

	// Token: 0x04000899 RID: 2201
	public AudioSource slave1;

	// Token: 0x0400089A RID: 2202
	public AudioSource slave2;

	// Token: 0x0400089B RID: 2203
	public AudioSource slave3;

	// Token: 0x0400089C RID: 2204
	public AudioSource slave4;
}
