using System;
using HutongGames.PlayMaker;

// Token: 0x02000381 RID: 897
[ActionCategory("Hollow Knight/GG")]
public class GGSetCanTransition : FSMUtility.SetBoolFsmStateAction
{
	// Token: 0x1700030C RID: 780
	// (set) Token: 0x06001E99 RID: 7833 RVA: 0x0008C6CF File Offset: 0x0008A8CF
	public override bool BoolValue
	{
		set
		{
			if (BossSceneController.Instance)
			{
				BossSceneController.Instance.CanTransition = value;
			}
		}
	}
}
