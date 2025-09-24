using System;
using UnityEngine;

// Token: 0x02000716 RID: 1814
public class ScoreBoardUIBadge : ScoreBoardUIBadgeBase
{
	// Token: 0x060040A0 RID: 16544 RVA: 0x0011C2BA File Offset: 0x0011A4BA
	private bool IsUsingConstant()
	{
		return !string.IsNullOrWhiteSpace(this.constantsInt);
	}

	// Token: 0x17000753 RID: 1875
	// (get) Token: 0x060040A1 RID: 16545 RVA: 0x0011C2CA File Offset: 0x0011A4CA
	public override int Score
	{
		get
		{
			if (!this.IsUsingConstant())
			{
				return this.score;
			}
			if (string.IsNullOrEmpty(this.constantsInt))
			{
				return 0;
			}
			return Constants.GetConstantValue<int>(this.constantsInt);
		}
	}

	// Token: 0x04004229 RID: 16937
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingConstant", false, true, false)]
	private int score;

	// Token: 0x0400422A RID: 16938
	[SerializeField]
	private string constantsInt;
}
