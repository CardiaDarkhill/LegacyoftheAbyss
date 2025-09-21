using System;
using GlobalEnums;
using HutongGames.PlayMaker;

// Token: 0x0200041E RID: 1054
[ActionCategory("Hollow Knight")]
public class CheckCurrentMapZoneEnum : FSMUtility.CheckFsmStateAction
{
	// Token: 0x060024ED RID: 9453 RVA: 0x000AA41F File Offset: 0x000A861F
	public override void Reset()
	{
		base.Reset();
		this.MapZone = null;
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x060024EE RID: 9454 RVA: 0x000AA42E File Offset: 0x000A862E
	public override bool IsTrue
	{
		get
		{
			return GameManager.instance && (MapZone)this.MapZone.Value == GameManager.instance.GetCurrentMapZoneEnum();
		}
	}

	// Token: 0x040022CD RID: 8909
	[RequiredField]
	[ObjectType(typeof(MapZone))]
	public FsmEnum MapZone;
}
