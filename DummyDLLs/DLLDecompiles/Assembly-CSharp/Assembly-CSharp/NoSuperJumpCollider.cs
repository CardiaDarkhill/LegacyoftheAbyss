using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class NoSuperJumpCollider : MonoBehaviour
{
	// Token: 0x060006FC RID: 1788 RVA: 0x00022FC6 File Offset: 0x000211C6
	private void Awake()
	{
		this.collider = base.GetComponent<Collider2D>();
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00022FD4 File Offset: 0x000211D4
	private void OnValidate()
	{
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00022FD6 File Offset: 0x000211D6
	private void OnEnable()
	{
		if (this.collider == null)
		{
			base.enabled = false;
			return;
		}
		NoSuperJumpCollider._insideZones.AddIfNotPresent(this);
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00022FFA File Offset: 0x000211FA
	private void OnDisable()
	{
		NoSuperJumpCollider._insideZones.Remove(this);
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00023008 File Offset: 0x00021208
	public static bool IsInside(Vector2 pos)
	{
		using (List<NoSuperJumpCollider>.Enumerator enumerator = NoSuperJumpCollider._insideZones.GetEnumerator())
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

	// Token: 0x040006DB RID: 1755
	private static readonly List<NoSuperJumpCollider> _insideZones = new List<NoSuperJumpCollider>();

	// Token: 0x040006DC RID: 1756
	private Collider2D collider;
}
