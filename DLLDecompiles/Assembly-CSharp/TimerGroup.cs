using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
[CreateAssetMenu(menuName = "Hornet/Timer Group")]
public class TimerGroup : ScriptableObject
{
	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06001352 RID: 4946 RVA: 0x00058693 File Offset: 0x00056893
	// (set) Token: 0x06001353 RID: 4947 RVA: 0x0005869B File Offset: 0x0005689B
	public double EndTime { get; private set; }

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06001354 RID: 4948 RVA: 0x000586A4 File Offset: 0x000568A4
	public float TimeLeft
	{
		get
		{
			return (float)(this.EndTime - Time.timeAsDouble);
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06001355 RID: 4949 RVA: 0x000586B3 File Offset: 0x000568B3
	public bool HasEnded
	{
		get
		{
			return Time.timeAsDouble >= this.EndTime;
		}
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x000586C5 File Offset: 0x000568C5
	public void ResetTimer()
	{
		this.EndTime = Time.timeAsDouble + (double)this.delay;
	}

	// Token: 0x040011BF RID: 4543
	[SerializeField]
	private float delay;
}
