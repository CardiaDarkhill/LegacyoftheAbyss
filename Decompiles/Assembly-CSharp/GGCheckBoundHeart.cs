using System;
using HutongGames.PlayMaker;

// Token: 0x0200038F RID: 911
[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundHeart : FSMUtility.CheckFsmStateAction
{
	// Token: 0x06001EE6 RID: 7910 RVA: 0x0008D43F File Offset: 0x0008B63F
	public override void Reset()
	{
		this.healthNumber = null;
		this.checkSource = GGCheckBoundHeart.CheckSource.Regular;
		base.Reset();
	}

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001EE7 RID: 7911 RVA: 0x0008D458 File Offset: 0x0008B658
	public override bool IsTrue
	{
		get
		{
			int num = -1;
			GGCheckBoundHeart.CheckSource checkSource = this.checkSource;
			if (checkSource != GGCheckBoundHeart.CheckSource.Regular)
			{
				if (checkSource == GGCheckBoundHeart.CheckSource.Joni)
				{
					num = (int)((float)this.healthNumber.Value * 0.71428573f) + 1;
				}
			}
			else
			{
				num = this.healthNumber.Value;
			}
			return BossSequenceController.BoundShell && num > BossSequenceController.BoundMaxHealth;
		}
	}

	// Token: 0x04001DB2 RID: 7602
	public FsmInt healthNumber;

	// Token: 0x04001DB3 RID: 7603
	public GGCheckBoundHeart.CheckSource checkSource;

	// Token: 0x02001634 RID: 5684
	public enum CheckSource
	{
		// Token: 0x040089FC RID: 35324
		Regular,
		// Token: 0x040089FD RID: 35325
		Joni
	}
}
