using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class SwampZone : DebugDrawColliderRuntimeAdder
{
	// Token: 0x060016BF RID: 5823 RVA: 0x0006650C File Offset: 0x0006470C
	protected override void Awake()
	{
		base.Awake();
		this.collider = base.GetComponent<Collider2D>();
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x00066520 File Offset: 0x00064720
	private void OnEnable()
	{
		SwampZone._activeZones.AddIfNotPresent(this);
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0006652E File Offset: 0x0006472E
	private void OnDisable()
	{
		SwampZone._activeZones.Remove(this);
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x0006653C File Offset: 0x0006473C
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x0006654C File Offset: 0x0006474C
	public static bool IsInside(Vector2 pos)
	{
		using (List<SwampZone>.Enumerator enumerator = SwampZone._activeZones.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.collider.OverlapPoint(pos))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04001537 RID: 5431
	private static readonly List<SwampZone> _activeZones = new List<SwampZone>();

	// Token: 0x04001538 RID: 5432
	private Collider2D collider;
}
