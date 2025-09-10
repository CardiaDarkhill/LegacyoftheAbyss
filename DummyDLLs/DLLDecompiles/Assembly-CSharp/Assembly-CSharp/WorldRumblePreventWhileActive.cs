using System;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class WorldRumblePreventWhileActive : MonoBehaviour, WorldRumbleManager.IWorldRumblePreventer
{
	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000BC8 RID: 3016 RVA: 0x0003591D File Offset: 0x00033B1D
	public bool AllowRumble
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00035920 File Offset: 0x00033B20
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		this.manager = silentInstance.worldRumbleManager;
		if (this.manager)
		{
			this.manager.AddPreventer(this);
		}
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0003596A File Offset: 0x00033B6A
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x00035979 File Offset: 0x00033B79
	private void OnDisable()
	{
		if (!this.manager)
		{
			return;
		}
		this.manager.RemovePreventer(this);
		this.manager = null;
	}

	// Token: 0x04000B5A RID: 2906
	private bool hasStarted;

	// Token: 0x04000B5B RID: 2907
	private WorldRumbleManager manager;
}
