using System;
using HutongGames.PlayMaker;

// Token: 0x0200042F RID: 1071
public class CheckHasVisitedScene : FSMUtility.CheckFsmStateAction
{
	// Token: 0x0600251C RID: 9500 RVA: 0x000AAB94 File Offset: 0x000A8D94
	public override void Reset()
	{
		base.Reset();
		this.SceneName = null;
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x0600251D RID: 9501 RVA: 0x000AABA3 File Offset: 0x000A8DA3
	public override bool IsTrue
	{
		get
		{
			return !string.IsNullOrEmpty(this.SceneName.Value) && PlayerData.instance.scenesVisited.Contains(this.SceneName.Value);
		}
	}

	// Token: 0x040022EE RID: 8942
	[RequiredField]
	public FsmString SceneName;
}
