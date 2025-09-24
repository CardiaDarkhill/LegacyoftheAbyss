using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000717 RID: 1815
public class ScoreBoardUIBadgeHero : ScoreBoardUIBadgeBase
{
	// Token: 0x17000754 RID: 1876
	// (get) Token: 0x060040A3 RID: 16547 RVA: 0x0011C2FD File Offset: 0x0011A4FD
	public override int Score
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0;
			}
			if (string.IsNullOrEmpty(this.scoreInt))
			{
				return 0;
			}
			return PlayerData.instance.GetVariable(this.scoreInt);
		}
	}

	// Token: 0x17000755 RID: 1877
	// (get) Token: 0x060040A4 RID: 16548 RVA: 0x0011C327 File Offset: 0x0011A527
	protected override bool IsVisible
	{
		get
		{
			return !Application.isPlaying || PlayerData.instance.GetVariable(this.playedBool);
		}
	}

	// Token: 0x0400422B RID: 16939
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string playedBool;

	// Token: 0x0400422C RID: 16940
	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string scoreInt;
}
