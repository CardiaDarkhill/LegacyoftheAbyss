using System;
using HutongGames.PlayMaker;

// Token: 0x020003AF RID: 943
[ActionCategory("Hollow Knight")]
public class ShowGodfinderIcon : FsmStateAction
{
	// Token: 0x06001FB3 RID: 8115 RVA: 0x00090E71 File Offset: 0x0008F071
	public override void Reset()
	{
		this.delay = null;
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x00090E7C File Offset: 0x0008F07C
	public override void OnEnter()
	{
		GodfinderIcon.ShowIcon(this.delay.Value, this.unlockBossScene.Value as BossScene);
		if (this.unlockBossScene.Value && !GameManager.instance.playerData.unlockedBossScenes.Contains(this.unlockBossScene.Value.name))
		{
			GameManager.instance.playerData.unlockedBossScenes.Add(this.unlockBossScene.Value.name);
		}
		base.Finish();
	}

	// Token: 0x04001EBE RID: 7870
	public FsmFloat delay;

	// Token: 0x04001EBF RID: 7871
	[ObjectType(typeof(BossScene))]
	public FsmObject unlockBossScene;
}
