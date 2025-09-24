using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000520 RID: 1312
[CreateAssetMenu(menuName = "Hornet/Localised Text Collection Config")]
public class NeedolinTextConfig : ScriptableObject
{
	// Token: 0x1700053C RID: 1340
	// (get) Token: 0x06002F2D RID: 12077 RVA: 0x000D043B File Offset: 0x000CE63B
	public float HoldDuration
	{
		get
		{
			return this.holdDuration;
		}
	}

	// Token: 0x1700053D RID: 1341
	// (get) Token: 0x06002F2E RID: 12078 RVA: 0x000D0443 File Offset: 0x000CE643
	public float SpeedMultiplier
	{
		get
		{
			return this.speedMultiplier;
		}
	}

	// Token: 0x1700053E RID: 1342
	// (get) Token: 0x06002F2F RID: 12079 RVA: 0x000D044B File Offset: 0x000CE64B
	public int EmptyStartGap
	{
		get
		{
			return this.emptyStartGap;
		}
	}

	// Token: 0x1700053F RID: 1343
	// (get) Token: 0x06002F30 RID: 12080 RVA: 0x000D0453 File Offset: 0x000CE653
	public MinMaxInt EmptyGap
	{
		get
		{
			return this.emptyGap;
		}
	}

	// Token: 0x040031FA RID: 12794
	[SerializeField]
	private float holdDuration = 0.6f;

	// Token: 0x040031FB RID: 12795
	[SerializeField]
	private float speedMultiplier = 1f;

	// Token: 0x040031FC RID: 12796
	[Space]
	[SerializeField]
	private int emptyStartGap;

	// Token: 0x040031FD RID: 12797
	[SerializeField]
	private MinMaxInt emptyGap;
}
