using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class RespawnMarker : MonoBehaviour
{
	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000711 RID: 1809 RVA: 0x000232FA File Offset: 0x000214FA
	// (set) Token: 0x06000712 RID: 1810 RVA: 0x00023301 File Offset: 0x00021501
	public static List<RespawnMarker> Markers { get; private set; }

	// Token: 0x06000713 RID: 1811 RVA: 0x00023309 File Offset: 0x00021509
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		RespawnMarker.Markers = new List<RespawnMarker>();
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00023315 File Offset: 0x00021515
	protected void Awake()
	{
		RespawnMarker.Markers.Add(this);
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x00023322 File Offset: 0x00021522
	protected void OnDestroy()
	{
		RespawnMarker.Markers.Remove(this);
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00023330 File Offset: 0x00021530
	public void PrepareRespawnHere()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "PREPARE HERO RESPAWNING HERE");
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00023342 File Offset: 0x00021542
	public void RespawnedHere()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "HERO RESPAWNING HERE");
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00023354 File Offset: 0x00021554
	public void SetCustomWakeUp(bool value)
	{
		this.customWakeUp = value;
	}

	// Token: 0x040006E2 RID: 1762
	public bool respawnFacingRight;

	// Token: 0x040006E3 RID: 1763
	public bool customWakeUp;

	// Token: 0x040006E4 RID: 1764
	[ModifiableProperty]
	[Conditional("customWakeUp", false, false, false)]
	public OverrideFloat customFadeDuration;

	// Token: 0x040006E5 RID: 1765
	public OverrideMapZone overrideMapZone;
}
