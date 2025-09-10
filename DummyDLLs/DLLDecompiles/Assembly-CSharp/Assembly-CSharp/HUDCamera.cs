using System;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class HUDCamera : MonoBehaviour
{
	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000B8D RID: 2957 RVA: 0x00035021 File Offset: 0x00033221
	public GameObject GameplayChild
	{
		get
		{
			return this.gameplayChild;
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x0003502C File Offset: 0x0003322C
	private void OnEnable()
	{
		if (!this.gc)
		{
			this.gc = GameCameras.instance;
		}
		if (!this.ih)
		{
			this.ih = GameManager.instance.inputHandler;
		}
		if (this.ih.PauseAllowed)
		{
			this.shouldEnablePause = true;
			this.ih.PreventPause();
		}
		else
		{
			this.shouldEnablePause = false;
		}
		base.Invoke("MoveMenuToHudCamera", 0.5f);
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x000350A6 File Offset: 0x000332A6
	private void MoveMenuToHudCamera()
	{
		this.gc.MoveMenuToHUDCamera();
		if (!this.shouldEnablePause)
		{
			return;
		}
		this.ih.AllowPause();
		this.shouldEnablePause = false;
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x000350CE File Offset: 0x000332CE
	public void SetIsGameplayMode(bool isGameplayMode)
	{
		this.gameplayChild.SetActive(isGameplayMode);
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x000350DC File Offset: 0x000332DC
	public void EnsureGameMapSpawned()
	{
		this.mapManager.EnsureMapsSpawned();
	}

	// Token: 0x04000B32 RID: 2866
	[SerializeField]
	private GameObject gameplayChild;

	// Token: 0x04000B33 RID: 2867
	[SerializeField]
	private InventoryMapManager mapManager;

	// Token: 0x04000B34 RID: 2868
	private GameCameras gc;

	// Token: 0x04000B35 RID: 2869
	private InputHandler ih;

	// Token: 0x04000B36 RID: 2870
	private bool shouldEnablePause;
}
