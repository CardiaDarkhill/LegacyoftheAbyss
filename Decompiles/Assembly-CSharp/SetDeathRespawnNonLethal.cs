using System;
using HutongGames.PlayMaker;

// Token: 0x02000426 RID: 1062
[ActionCategory("Hollow Knight")]
public class SetDeathRespawnNonLethal : FsmStateAction
{
	// Token: 0x06002505 RID: 9477 RVA: 0x000AA924 File Offset: 0x000A8B24
	public override void Reset()
	{
		this.gameManager = new FsmGameObject();
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000AA934 File Offset: 0x000A8B34
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.SetNonlethalDeathRespawn(this.respawnMarkerName.Value, this.respawnType.Value, this.respawnFacingRight.Value);
		}
		base.Finish();
	}

	// Token: 0x040022E4 RID: 8932
	[UIHint(UIHint.Variable)]
	public FsmGameObject gameManager;

	// Token: 0x040022E5 RID: 8933
	public FsmString respawnMarkerName;

	// Token: 0x040022E6 RID: 8934
	public FsmInt respawnType;

	// Token: 0x040022E7 RID: 8935
	public FsmBool respawnFacingRight;
}
