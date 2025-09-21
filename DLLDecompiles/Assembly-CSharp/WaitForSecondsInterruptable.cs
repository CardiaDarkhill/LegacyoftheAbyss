using System;
using UnityEngine;

// Token: 0x0200079B RID: 1947
public class WaitForSecondsInterruptable : CustomYieldInstruction
{
	// Token: 0x170007B2 RID: 1970
	// (get) Token: 0x060044BB RID: 17595 RVA: 0x0012CD2C File Offset: 0x0012AF2C
	public override bool keepWaiting
	{
		get
		{
			return (this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble) < this.endTime && !this.endCondition();
		}
	}

	// Token: 0x060044BC RID: 17596 RVA: 0x0012CD5A File Offset: 0x0012AF5A
	public WaitForSecondsInterruptable(float seconds, Func<bool> endCondition, bool isRealtime = false)
	{
		this.isRealtime = isRealtime;
		this.seconds = seconds;
		this.endCondition = endCondition;
		this.ResetTimer();
	}

	// Token: 0x060044BD RID: 17597 RVA: 0x0012CD80 File Offset: 0x0012AF80
	public void ResetTimer()
	{
		double num = this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;
		this.endTime = num + (double)this.seconds;
	}

	// Token: 0x040045AA RID: 17834
	private readonly float seconds;

	// Token: 0x040045AB RID: 17835
	private double endTime;

	// Token: 0x040045AC RID: 17836
	private readonly Func<bool> endCondition;

	// Token: 0x040045AD RID: 17837
	private readonly bool isRealtime;
}
