using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000790 RID: 1936
public class TimeScaleIndependentUpdate : MonoBehaviour
{
	// Token: 0x170007AC RID: 1964
	// (get) Token: 0x0600448C RID: 17548 RVA: 0x0012C514 File Offset: 0x0012A714
	// (set) Token: 0x0600448D RID: 17549 RVA: 0x0012C51C File Offset: 0x0012A71C
	public float deltaTime { get; private set; }

	// Token: 0x0600448E RID: 17550 RVA: 0x0012C525 File Offset: 0x0012A725
	protected virtual void Awake()
	{
		this.previousTimeSinceStartup = Time.realtimeSinceStartup;
	}

	// Token: 0x0600448F RID: 17551 RVA: 0x0012C534 File Offset: 0x0012A734
	protected virtual void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		this.deltaTime = realtimeSinceStartup - this.previousTimeSinceStartup;
		this.previousTimeSinceStartup = realtimeSinceStartup;
		if (this.deltaTime < 0f)
		{
			this.deltaTime = 0f;
		}
	}

	// Token: 0x06004490 RID: 17552 RVA: 0x0012C574 File Offset: 0x0012A774
	public IEnumerator TimeScaleIndependentWaitForSeconds(float seconds)
	{
		for (float elapsedTime = 0f; elapsedTime < seconds; elapsedTime += this.deltaTime)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x04004593 RID: 17811
	private float previousTimeSinceStartup;
}
