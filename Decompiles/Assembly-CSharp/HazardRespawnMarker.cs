using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[ExecuteInEditMode]
public class HazardRespawnMarker : MonoBehaviour
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x060006D1 RID: 1745 RVA: 0x0002265F File Offset: 0x0002085F
	public HazardRespawnMarker.FacingDirection RespawnFacingDirection
	{
		get
		{
			return this.respawnFacingDirection;
		}
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00022667 File Offset: 0x00020867
	private void OnValidate()
	{
		if (this.respawnFacingRight)
		{
			this.respawnFacingDirection = HazardRespawnMarker.FacingDirection.Right;
			this.respawnFacingRight = false;
		}
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00022680 File Offset: 0x00020880
	private void Awake()
	{
		this.OnValidate();
		if (base.transform.parent != null && base.transform.parent.name.Contains("top"))
		{
			this.groundSenseDistance = 50f;
		}
		this.heroSpawnLocation = Helper.Raycast2D(base.transform.position, this.groundSenseRay, this.groundSenseDistance, 256).point;
	}

	// Token: 0x040006AE RID: 1710
	[SerializeField]
	private HazardRespawnMarker.FacingDirection respawnFacingDirection;

	// Token: 0x040006AF RID: 1711
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool respawnFacingRight;

	// Token: 0x040006B0 RID: 1712
	private float groundSenseDistance = 3f;

	// Token: 0x040006B1 RID: 1713
	private readonly Vector2 groundSenseRay = -Vector2.up;

	// Token: 0x040006B2 RID: 1714
	private Vector2 heroSpawnLocation;

	// Token: 0x040006B3 RID: 1715
	public bool drawDebugRays;

	// Token: 0x02001446 RID: 5190
	public enum FacingDirection
	{
		// Token: 0x040082AA RID: 33450
		None,
		// Token: 0x040082AB RID: 33451
		Left,
		// Token: 0x040082AC RID: 33452
		Right
	}
}
