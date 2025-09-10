using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020001A5 RID: 421
[Serializable]
public class Achievement
{
	// Token: 0x17000197 RID: 407
	// (get) Token: 0x0600104E RID: 4174 RVA: 0x0004E809 File Offset: 0x0004CA09
	public string TitleCell
	{
		get
		{
			return this.PlatformKey + "_NAME";
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x0600104F RID: 4175 RVA: 0x0004E81B File Offset: 0x0004CA1B
	public string DescriptionCell
	{
		get
		{
			return this.PlatformKey + "_DESC";
		}
	}

	// Token: 0x04000FCA RID: 4042
	public string PlatformKey;

	// Token: 0x04000FCB RID: 4043
	public AchievementType Type;

	// Token: 0x04000FCC RID: 4044
	public Sprite Icon;
}
