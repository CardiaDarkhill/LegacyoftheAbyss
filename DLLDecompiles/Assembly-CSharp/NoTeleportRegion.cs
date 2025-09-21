using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class NoTeleportRegion : DebugDrawColliderRuntimeAdder
{
	// Token: 0x06000703 RID: 1795 RVA: 0x0002307C File Offset: 0x0002127C
	private new void Awake()
	{
		this.collider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0002308A File Offset: 0x0002128A
	private void OnEnable()
	{
		NoTeleportRegion._activeRegions.AddIfNotPresent(this);
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00023098 File Offset: 0x00021298
	private void OnDisable()
	{
		NoTeleportRegion._activeRegions.Remove(this);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x000230A6 File Offset: 0x000212A6
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x000230B8 File Offset: 0x000212B8
	public static NoTeleportRegion.TeleportAllowState GetTeleportBlockedState(Vector2 pos)
	{
		GameManager instance = GameManager.instance;
		CustomSceneManager customSceneManager = instance ? instance.sm : null;
		if (customSceneManager == null)
		{
			return NoTeleportRegion.TeleportAllowState.Blocked;
		}
		foreach (NoTeleportRegion noTeleportRegion in NoTeleportRegion._activeRegions)
		{
			if (noTeleportRegion.allowState == NoTeleportRegion.TeleportAllowState.Blocked && noTeleportRegion.collider.OverlapPoint(pos))
			{
				return NoTeleportRegion.TeleportAllowState.Blocked;
			}
		}
		if (ToolItemManager.ActiveState != ToolsActiveStates.Active)
		{
			return NoTeleportRegion.TeleportAllowState.Blocked;
		}
		foreach (NoTeleportRegion noTeleportRegion2 in NoTeleportRegion._activeRegions)
		{
			if (noTeleportRegion2.allowState == NoTeleportRegion.TeleportAllowState.Allowed && noTeleportRegion2.collider.OverlapPoint(pos))
			{
				return NoTeleportRegion.TeleportAllowState.Allowed;
			}
		}
		MapZone mapZone = customSceneManager.mapZone;
		if (mapZone == MapZone.DUST_MAZE || mapZone == MapZone.ABYSS)
		{
			return NoTeleportRegion.TeleportAllowState.Blocked;
		}
		if (GameManager.IsMemoryScene(customSceneManager.mapZone))
		{
			return NoTeleportRegion.TeleportAllowState.Blocked;
		}
		switch (customSceneManager.TeleportAllowState)
		{
		case NoTeleportRegion.TeleportAllowState.Standard:
		{
			bool flag;
			instance.gameMap.HasMapForScene(instance.GetSceneNameString(), out flag);
			if (!flag)
			{
				return NoTeleportRegion.TeleportAllowState.Blocked;
			}
			return NoTeleportRegion.TeleportAllowState.Standard;
		}
		case NoTeleportRegion.TeleportAllowState.Blocked:
			return NoTeleportRegion.TeleportAllowState.Blocked;
		case NoTeleportRegion.TeleportAllowState.Allowed:
			return NoTeleportRegion.TeleportAllowState.Standard;
		default:
			throw new ArgumentOutOfRangeException();
		}
		NoTeleportRegion.TeleportAllowState result;
		return result;
	}

	// Token: 0x040006DD RID: 1757
	[Space]
	[SerializeField]
	private NoTeleportRegion.TeleportAllowState allowState = NoTeleportRegion.TeleportAllowState.Blocked;

	// Token: 0x040006DE RID: 1758
	private static readonly List<NoTeleportRegion> _activeRegions = new List<NoTeleportRegion>();

	// Token: 0x040006DF RID: 1759
	private Collider2D collider;

	// Token: 0x02001449 RID: 5193
	public enum TeleportAllowState
	{
		// Token: 0x040082B8 RID: 33464
		Standard,
		// Token: 0x040082B9 RID: 33465
		Blocked,
		// Token: 0x040082BA RID: 33466
		Allowed
	}
}
