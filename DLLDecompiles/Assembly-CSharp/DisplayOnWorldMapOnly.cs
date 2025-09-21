using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000644 RID: 1604
public class DisplayOnWorldMapOnly : MonoBehaviour
{
	// Token: 0x0600398C RID: 14732 RVA: 0x000FCBDF File Offset: 0x000FADDF
	private void Reset()
	{
		this.gameMap = base.GetComponentInParent<GameMap>(true);
	}

	// Token: 0x0600398D RID: 14733 RVA: 0x000FCBEE File Offset: 0x000FADEE
	private void Awake()
	{
		this.gameMap = base.GetComponentInParent<GameMap>();
		this.gameMap.UpdateQuickMapDisplay += this.Refresh;
		this.renderer = base.GetComponent<Renderer>();
		this.parentScene = base.GetComponentInParent<GameMapScene>(true);
	}

	// Token: 0x0600398E RID: 14734 RVA: 0x000FCC2C File Offset: 0x000FAE2C
	private void Start()
	{
		if (DisplayOnWorldMapOnly.updateState != DisplayOnWorldMapOnly.UpdateState.Never)
		{
			this.Refresh(DisplayOnWorldMapOnly.updateState == DisplayOnWorldMapOnly.UpdateState.QuickMap, MapZone.NONE);
		}
	}

	// Token: 0x0600398F RID: 14735 RVA: 0x000FCC44 File Offset: 0x000FAE44
	private void OnDestroy()
	{
		this.gameMap.UpdateQuickMapDisplay -= this.Refresh;
	}

	// Token: 0x06003990 RID: 14736 RVA: 0x000FCC60 File Offset: 0x000FAE60
	private void Refresh(bool isQuickMap, MapZone _)
	{
		DisplayOnWorldMapOnly.updateState = (isQuickMap ? DisplayOnWorldMapOnly.UpdateState.QuickMap : DisplayOnWorldMapOnly.UpdateState.Normal);
		if (!this.renderer)
		{
			return;
		}
		this.renderer.enabled = (!isQuickMap && (!this.parentScene || this.parentScene.IsMapped || this.parentScene.InitialState > GameMapScene.States.Hidden));
	}

	// Token: 0x04003C46 RID: 15430
	[SerializeField]
	private GameMap gameMap;

	// Token: 0x04003C47 RID: 15431
	private GameMapScene parentScene;

	// Token: 0x04003C48 RID: 15432
	private Renderer renderer;

	// Token: 0x04003C49 RID: 15433
	private bool hasEverRefreshed;

	// Token: 0x04003C4A RID: 15434
	private static DisplayOnWorldMapOnly.UpdateState updateState;

	// Token: 0x02001962 RID: 6498
	private enum UpdateState
	{
		// Token: 0x04009593 RID: 38291
		Never,
		// Token: 0x04009594 RID: 38292
		Normal,
		// Token: 0x04009595 RID: 38293
		QuickMap
	}
}
