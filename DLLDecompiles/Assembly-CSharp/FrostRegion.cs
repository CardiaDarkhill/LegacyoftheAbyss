using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class FrostRegion : TrackTriggerObjects
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060006C6 RID: 1734 RVA: 0x000225B3 File Offset: 0x000207B3
	public float FrostSpeed
	{
		get
		{
			if (!this.IsUsingProfile())
			{
				return this.frostSpeed;
			}
			return this.frostSpeedProfile.FrostSpeed;
		}
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x000225CF File Offset: 0x000207CF
	private bool IsUsingProfile()
	{
		return this.frostSpeedProfile;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x000225DC File Offset: 0x000207DC
	protected override void OnEnable()
	{
		base.OnEnable();
		FrostRegion._activeRegions.Add(this);
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x000225EF File Offset: 0x000207EF
	protected override void OnDisable()
	{
		base.OnDisable();
		FrostRegion._activeRegions.Remove(this);
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00022603 File Offset: 0x00020803
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (!isInside)
		{
			return;
		}
		if (Mathf.Abs(this.addOnEnter) > Mathf.Epsilon)
		{
			HeroController.instance.AddFrost(this.addOnEnter);
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x0002262B File Offset: 0x0002082B
	public static IEnumerable<FrostRegion> EnumerateInsideRegions()
	{
		foreach (FrostRegion frostRegion in FrostRegion._activeRegions)
		{
			if (frostRegion.IsInside)
			{
				yield return frostRegion;
			}
		}
		List<FrostRegion>.Enumerator enumerator = default(List<FrostRegion>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x00022634 File Offset: 0x00020834
	public static List<FrostRegion> FrostRegions
	{
		get
		{
			return FrostRegion._activeRegions;
		}
	}

	// Token: 0x0400069F RID: 1695
	public const float BASE_WARMTH = 100f;

	// Token: 0x040006A0 RID: 1696
	public const float FROST_SPEED_MULT = 1f;

	// Token: 0x040006A1 RID: 1697
	public const float CLOAKLESS_FROST_MULT = 2f;

	// Token: 0x040006A2 RID: 1698
	public const float FEATHERED_CLOAK_CAP = 0.035f;

	// Token: 0x040006A3 RID: 1699
	public const float HERO_DAMAGE_TIME = 1.75f;

	// Token: 0x040006A4 RID: 1700
	public const float FIRE_IMBUEMENT_ADD_FROST = -25f;

	// Token: 0x040006A5 RID: 1701
	public const float FIRE_IMBUEMENT_MULTIPLIER = 0.7f;

	// Token: 0x040006A6 RID: 1702
	public const float WARRIOR_RAGE_ADD_FROST = -15f;

	// Token: 0x040006A7 RID: 1703
	public const float WARRIOR_RAGE_MULTIPLIER = 0.9f;

	// Token: 0x040006A8 RID: 1704
	public const float WISP_LANTERN_MULTIPLIER = 0.8f;

	// Token: 0x040006A9 RID: 1705
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingProfile", false, true, false)]
	private float frostSpeed;

	// Token: 0x040006AA RID: 1706
	[SerializeField]
	[AssetPickerDropdown]
	private FrostSpeedProfile frostSpeedProfile;

	// Token: 0x040006AB RID: 1707
	[SerializeField]
	[Range(-100f, 100f)]
	private float addOnEnter;

	// Token: 0x040006AC RID: 1708
	private static readonly List<FrostRegion> _activeRegions = new List<FrostRegion>();
}
