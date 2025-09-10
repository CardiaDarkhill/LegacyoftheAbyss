using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000433 RID: 1075
public class CheckIsRespawningOnMarker : FSMUtility.CheckFsmStateAction
{
	// Token: 0x06002528 RID: 9512 RVA: 0x000AACD4 File Offset: 0x000A8ED4
	public override void Reset()
	{
		base.Reset();
		this.MarkerObject = null;
	}

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06002529 RID: 9513 RVA: 0x000AACE4 File Offset: 0x000A8EE4
	public override bool IsTrue
	{
		get
		{
			GameObject safe = this.MarkerObject.GetSafe(this);
			if (!safe)
			{
				return false;
			}
			string name = safe.name;
			GameManager instance = GameManager.instance;
			PlayerData instance2 = PlayerData.instance;
			return !(name != instance2.respawnMarkerName) && !(instance.GetSceneNameString() != instance2.respawnScene);
		}
	}

	// Token: 0x040022F3 RID: 8947
	public FsmOwnerDefault MarkerObject;
}
