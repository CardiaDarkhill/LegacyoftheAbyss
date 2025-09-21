using System;

// Token: 0x020005E7 RID: 1511
public static class ToolItemTypeExtensions
{
	// Token: 0x060035DA RID: 13786 RVA: 0x000ED6D2 File Offset: 0x000EB8D2
	public static bool IsAttackType(this ToolItemType type)
	{
		return type == ToolItemType.Red || type == ToolItemType.Skill;
	}
}
