using System;
using HutongGames.PlayMaker;

// Token: 0x0200037C RID: 892
[ActionCategory("Hollow Knight")]
public class GGCheckBossSceneUnlocked : FSMUtility.CheckFsmStateAction
{
	// Token: 0x06001E6E RID: 7790 RVA: 0x0008C126 File Offset: 0x0008A326
	public override void Reset()
	{
		base.Reset();
		this.bossScene = null;
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001E6F RID: 7791 RVA: 0x0008C135 File Offset: 0x0008A335
	public override bool IsTrue
	{
		get
		{
			return !this.bossScene.IsNone && ((BossScene)this.bossScene.Value).IsUnlocked(this.checkSource);
		}
	}

	// Token: 0x04001D74 RID: 7540
	[ObjectType(typeof(BossScene))]
	public FsmObject bossScene;

	// Token: 0x04001D75 RID: 7541
	public BossSceneCheckSource checkSource;
}
