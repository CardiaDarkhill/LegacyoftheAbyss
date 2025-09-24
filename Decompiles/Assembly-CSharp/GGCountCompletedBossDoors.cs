using System;
using System.Reflection;
using HutongGames.PlayMaker;

// Token: 0x02000396 RID: 918
[ActionCategory("Hollow Knight")]
public class GGCountCompletedBossDoors : FSMUtility.GetIntFsmStateAction
{
	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001F01 RID: 7937 RVA: 0x0008DAE8 File Offset: 0x0008BCE8
	public override int IntValue
	{
		get
		{
			int num = 0;
			foreach (FieldInfo fieldInfo in typeof(PlayerData).GetFields())
			{
				if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion) && ((BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData)).completed)
				{
					num++;
				}
			}
			return num;
		}
	}
}
