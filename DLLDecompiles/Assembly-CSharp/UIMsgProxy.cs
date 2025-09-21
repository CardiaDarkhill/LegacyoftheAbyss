using System;
using UnityEngine;

// Token: 0x02000745 RID: 1861
public class UIMsgProxy : MonoBehaviour, WorldRumbleManager.IWorldRumblePreventer
{
	// Token: 0x17000783 RID: 1923
	// (get) Token: 0x06004253 RID: 16979 RVA: 0x00124F14 File Offset: 0x00123114
	public bool AllowRumble
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06004254 RID: 16980 RVA: 0x00124F18 File Offset: 0x00123118
	public void SetIsInMsg(bool value)
	{
		WorldRumbleManager worldRumbleManager = GameCameras.SilentInstance.worldRumbleManager;
		if (value)
		{
			worldRumbleManager.AddPreventer(this);
			this.wasDisablePause = PlayerData.instance.disablePause;
			PlayerData.instance.disablePause = true;
			CollectableItemPickup.IsPickupPaused = true;
			InventoryPaneInput.IsInputBlocked = true;
			return;
		}
		worldRumbleManager.RemovePreventer(this);
		CollectableItemPickup.IsPickupPaused = false;
		InventoryPaneInput.IsInputBlocked = false;
		PlayerData.instance.disablePause = this.wasDisablePause;
	}

	// Token: 0x040043ED RID: 17389
	private bool wasDisablePause;
}
