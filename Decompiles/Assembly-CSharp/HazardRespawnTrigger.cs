using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class HazardRespawnTrigger : MonoBehaviour
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00022724 File Offset: 0x00020924
	// (set) Token: 0x060006D6 RID: 1750 RVA: 0x0002272B File Offset: 0x0002092B
	public static bool IsSuppressed { get; set; }

	// Token: 0x060006D7 RID: 1751 RVA: 0x00022733 File Offset: 0x00020933
	private void Awake()
	{
		this.playerData = PlayerData.instance;
		if (this.playerData == null)
		{
			Debug.LogError(base.name + "- Player Data reference is null, please check this is being set correctly.");
		}
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x0002275D File Offset: 0x0002095D
	private void Start()
	{
		if (this.respawnMarker == null)
		{
			Debug.LogWarning(base.name + " does not have a Hazard Respawn Marker Set");
		}
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00022784 File Offset: 0x00020984
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (this.inactive || HazardRespawnTrigger.IsSuppressed)
		{
			return;
		}
		if (otherCollider.gameObject.layer != 9)
		{
			return;
		}
		this.playerData.SetHazardRespawn(this.respawnMarker);
		if (this.fireOnce)
		{
			this.inactive = true;
		}
	}

	// Token: 0x040006B4 RID: 1716
	public HazardRespawnMarker respawnMarker;

	// Token: 0x040006B5 RID: 1717
	private PlayerData playerData;

	// Token: 0x040006B6 RID: 1718
	public bool fireOnce;

	// Token: 0x040006B7 RID: 1719
	private bool inactive;
}
