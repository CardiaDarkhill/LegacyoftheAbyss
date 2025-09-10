using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2E RID: 3374
	[ActionCategory(ActionCategory.GameObject)]
	public class SetHazardRespawn : FsmStateAction
	{
		// Token: 0x06006353 RID: 25427 RVA: 0x001F5F1B File Offset: 0x001F411B
		public override void Reset()
		{
			this.hazardRespawnMarker = null;
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x001F5F24 File Offset: 0x001F4124
		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			if (instance == null)
			{
				Debug.LogError("Player Data reference is null, please check this is being set correctly.");
			}
			HazardRespawnMarker component = this.hazardRespawnMarker.Value.GetComponent<HazardRespawnMarker>();
			if (component)
			{
				instance.SetHazardRespawn(component);
			}
			base.Finish();
		}

		// Token: 0x040061B8 RID: 25016
		[RequiredField]
		public FsmGameObject hazardRespawnMarker;
	}
}
