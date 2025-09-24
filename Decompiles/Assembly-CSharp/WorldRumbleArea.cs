using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class WorldRumbleArea : TrackTriggerObjects, WorldRumbleManager.IWorldRumblePreventer
{
	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x000358D7 File Offset: 0x00033AD7
	public bool AllowRumble
	{
		get
		{
			return this.allowRumble;
		}
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x000358E0 File Offset: 0x00033AE0
	protected override void OnInsideStateChanged(bool isInside)
	{
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		WorldRumbleManager worldRumbleManager = silentInstance.worldRumbleManager;
		if (isInside)
		{
			worldRumbleManager.AddPreventer(this);
			return;
		}
		worldRumbleManager.RemovePreventer(this);
	}

	// Token: 0x04000B59 RID: 2905
	[Space]
	[SerializeField]
	private bool allowRumble;
}
