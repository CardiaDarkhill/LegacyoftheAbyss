using System;
using HutongGames.PlayMaker;

// Token: 0x02000423 RID: 1059
[ActionCategory("Hollow Knight")]
public class SetDeathRespawn : FsmStateAction
{
	// Token: 0x060024FC RID: 9468 RVA: 0x000AA6AD File Offset: 0x000A88AD
	public override void Reset()
	{
		this.gameManager = new FsmGameObject();
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000AA6BC File Offset: 0x000A88BC
	public override void OnEnter()
	{
		if (this.gameManager.Value != null)
		{
			GameManager component = this.gameManager.Value.GetComponent<GameManager>();
			if (component != null)
			{
				component.SetDeathRespawnSimple(this.respawnMarkerName.Value, this.respawnType.Value, this.respawnFacingRight.Value);
			}
			base.Finish();
		}
	}

	// Token: 0x040022D5 RID: 8917
	[UIHint(UIHint.Variable)]
	public FsmGameObject gameManager;

	// Token: 0x040022D6 RID: 8918
	public FsmString respawnMarkerName;

	// Token: 0x040022D7 RID: 8919
	public FsmInt respawnType;

	// Token: 0x040022D8 RID: 8920
	public FsmBool respawnFacingRight;
}
