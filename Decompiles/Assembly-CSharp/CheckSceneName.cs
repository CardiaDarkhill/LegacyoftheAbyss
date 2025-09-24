using System;
using HutongGames.PlayMaker;

// Token: 0x0200041F RID: 1055
[ActionCategory("Hollow Knight")]
public class CheckSceneName : FsmStateAction
{
	// Token: 0x060024F0 RID: 9456 RVA: 0x000AA462 File Offset: 0x000A8662
	public override void Reset()
	{
		this.sceneName = null;
		this.equalEvent = null;
		this.notEqualEvent = null;
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x000AA479 File Offset: 0x000A8679
	public override void Awake()
	{
		this.sceneNameHash = this.sceneName.Value.GetHashCode();
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x000AA494 File Offset: 0x000A8694
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			bool flag = this.sceneName.UsesVariable ? (this.sceneName.Value == instance.GetSceneNameString()) : (this.sceneNameHash == instance.sceneNameHash);
			base.Fsm.Event(flag ? this.equalEvent : this.notEqualEvent);
		}
		base.Finish();
	}

	// Token: 0x040022CE RID: 8910
	[RequiredField]
	public FsmString sceneName;

	// Token: 0x040022CF RID: 8911
	public FsmEvent equalEvent;

	// Token: 0x040022D0 RID: 8912
	public FsmEvent notEqualEvent;

	// Token: 0x040022D1 RID: 8913
	private int sceneNameHash;
}
