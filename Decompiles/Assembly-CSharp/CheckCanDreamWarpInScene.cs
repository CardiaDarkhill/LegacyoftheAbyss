using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;

// Token: 0x02000422 RID: 1058
[ActionCategory("Hollow Knight")]
public class CheckCanDreamWarpInScene : FSMUtility.CheckFsmStateAction
{
	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x060024F9 RID: 9465 RVA: 0x000AA5A8 File Offset: 0x000A87A8
	public override bool IsTrue
	{
		get
		{
			string sceneNameString = GameManager.instance.GetSceneNameString();
			return !this.sceneCheckFunctions.ContainsKey(sceneNameString) || this.sceneCheckFunctions[sceneNameString]();
		}
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000AA5E4 File Offset: 0x000A87E4
	public CheckCanDreamWarpInScene()
	{
		Dictionary<string, Func<bool>> dictionary = new Dictionary<string, Func<bool>>();
		dictionary.Add("GG_Atrium", CheckCanDreamWarpInScene.bossRushCheck);
		dictionary.Add("GG_Atrium_Roof", CheckCanDreamWarpInScene.bossRushCheck);
		dictionary.Add("GG_Workshop", CheckCanDreamWarpInScene.bossRushCheck);
		dictionary.Add("GG_Blue_Room", CheckCanDreamWarpInScene.bossRushCheck);
		dictionary.Add("GG_Land_of_Storms", () => false);
		dictionary.Add("GG_Unlock_Wastes", () => false);
		this.sceneCheckFunctions = dictionary;
		base..ctor();
	}

	// Token: 0x040022D3 RID: 8915
	private static Func<bool> bossRushCheck = () => !GameManager.instance.playerData.bossRushMode;

	// Token: 0x040022D4 RID: 8916
	private Dictionary<string, Func<bool>> sceneCheckFunctions;
}
