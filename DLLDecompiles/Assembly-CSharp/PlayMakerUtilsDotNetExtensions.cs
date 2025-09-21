using System;
using HutongGames.PlayMaker;

// Token: 0x02000041 RID: 65
public static class PlayMakerUtilsDotNetExtensions
{
	// Token: 0x060001CE RID: 462 RVA: 0x0000972C File Offset: 0x0000792C
	public static bool Contains(this VariableType[] target, VariableType vType)
	{
		if (target == null)
		{
			return false;
		}
		for (int i = 0; i < target.Length; i++)
		{
			if (target[i] == vType)
			{
				return true;
			}
		}
		return false;
	}
}
