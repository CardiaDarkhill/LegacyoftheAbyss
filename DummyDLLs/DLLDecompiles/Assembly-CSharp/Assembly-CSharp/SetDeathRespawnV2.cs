using System;
using GlobalEnums;
using HutongGames.PlayMaker;

// Token: 0x02000424 RID: 1060
[ActionCategory("Hollow Knight")]
public class SetDeathRespawnV2 : FsmStateAction
{
	// Token: 0x060024FF RID: 9471 RVA: 0x000AA72B File Offset: 0x000A892B
	public override void Reset()
	{
		this.RespawnMarkerName = null;
		this.RespawnType = null;
		this.RespawnFacingRight = null;
		this.RespawnMapZone = new FsmEnum
		{
			UseVariable = true
		};
		this.RespawnExtraRestZone = new FsmEnum
		{
			UseVariable = true
		};
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000AA768 File Offset: 0x000A8968
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		PlayerData playerData = instance.playerData;
		instance.SetDeathRespawnSimple(this.RespawnMarkerName.Value, this.RespawnType.Value, this.RespawnFacingRight.Value);
		if (!this.RespawnMapZone.IsNone)
		{
			playerData.mapZone = (MapZone)this.RespawnMapZone.Value;
		}
		if (!this.RespawnExtraRestZone.IsNone)
		{
			playerData.extraRestZone = (ExtraRestZones)this.RespawnExtraRestZone.Value;
		}
		base.Finish();
	}

	// Token: 0x040022D9 RID: 8921
	public FsmString RespawnMarkerName;

	// Token: 0x040022DA RID: 8922
	public FsmInt RespawnType;

	// Token: 0x040022DB RID: 8923
	public FsmBool RespawnFacingRight;

	// Token: 0x040022DC RID: 8924
	[ObjectType(typeof(MapZone))]
	public FsmEnum RespawnMapZone;

	// Token: 0x040022DD RID: 8925
	[ObjectType(typeof(ExtraRestZones))]
	public FsmEnum RespawnExtraRestZone;
}
