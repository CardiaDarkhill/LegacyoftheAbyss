using System;
using GlobalEnums;
using HutongGames.PlayMaker;

// Token: 0x02000425 RID: 1061
[ActionCategory("Hollow Knight")]
public class SetDeathRespawnOtherScene : FsmStateAction
{
	// Token: 0x06002502 RID: 9474 RVA: 0x000AA7FC File Offset: 0x000A89FC
	public override void Reset()
	{
		this.RespawnSceneName = null;
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

	// Token: 0x06002503 RID: 9475 RVA: 0x000AA84C File Offset: 0x000A8A4C
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		PlayerData playerData = instance.playerData;
		instance.SetDeathRespawnSimple(this.RespawnMarkerName.Value, this.RespawnType.Value, this.RespawnFacingRight.Value);
		if (!this.RespawnSceneName.IsNone)
		{
			playerData.respawnScene = this.RespawnSceneName.Value;
		}
		if (!this.RespawnMapZone.IsNone)
		{
			playerData.mapZone = (MapZone)this.RespawnMapZone.Value;
		}
		else
		{
			SceneTeleportMap.SceneInfo sceneInfo = SceneTeleportMap.GetTeleportMap()[this.RespawnSceneName.Value];
			if (sceneInfo != null)
			{
				playerData.mapZone = sceneInfo.MapZone;
			}
		}
		if (!this.RespawnExtraRestZone.IsNone)
		{
			playerData.extraRestZone = (ExtraRestZones)this.RespawnExtraRestZone.Value;
		}
		base.Finish();
	}

	// Token: 0x040022DE RID: 8926
	public FsmString RespawnSceneName;

	// Token: 0x040022DF RID: 8927
	public FsmString RespawnMarkerName;

	// Token: 0x040022E0 RID: 8928
	public FsmInt RespawnType;

	// Token: 0x040022E1 RID: 8929
	public FsmBool RespawnFacingRight;

	// Token: 0x040022E2 RID: 8930
	[ObjectType(typeof(MapZone))]
	public FsmEnum RespawnMapZone;

	// Token: 0x040022E3 RID: 8931
	[ObjectType(typeof(ExtraRestZones))]
	public FsmEnum RespawnExtraRestZone;
}
