using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000630 RID: 1584
public sealed class HudScalePositioner : MonoBehaviour
{
	// Token: 0x1700066E RID: 1646
	// (get) Token: 0x0600387D RID: 14461 RVA: 0x000F98F6 File Offset: 0x000F7AF6
	// (set) Token: 0x0600387E RID: 14462 RVA: 0x000F9900 File Offset: 0x000F7B00
	public static bool IsReduced
	{
		get
		{
			return HudScalePositioner._isReduced;
		}
		set
		{
			HudScalePositioner._isReduced = value;
			foreach (HudScalePositioner hudScalePositioner in HudScalePositioner._activeObjs)
			{
				hudScalePositioner.UpdatePosition();
			}
		}
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x000F9958 File Offset: 0x000F7B58
	private void OnEnable()
	{
		this.UpdatePosition();
		HudScalePositioner._activeObjs.Add(this);
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x000F996C File Offset: 0x000F7B6C
	private void OnDisable()
	{
		HudScalePositioner._activeObjs.Remove(this);
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x000F997A File Offset: 0x000F7B7A
	private void UpdatePosition()
	{
		if (HudScalePositioner.IsReduced)
		{
			base.transform.localPosition = this.reducedPosition;
			return;
		}
		base.transform.localPosition = this.largePosition;
	}

	// Token: 0x04003B70 RID: 15216
	[SerializeField]
	private Vector3 reducedPosition;

	// Token: 0x04003B71 RID: 15217
	[SerializeField]
	private Vector3 largePosition;

	// Token: 0x04003B72 RID: 15218
	private static HashSet<HudScalePositioner> _activeObjs = new HashSet<HudScalePositioner>();

	// Token: 0x04003B73 RID: 15219
	private static bool _isReduced;
}
