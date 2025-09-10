using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000672 RID: 1650
public class HudGlobalHide : MonoBehaviour
{
	// Token: 0x170006AE RID: 1710
	// (get) Token: 0x06003B2A RID: 15146 RVA: 0x00104AB2 File Offset: 0x00102CB2
	// (set) Token: 0x06003B2B RID: 15147 RVA: 0x00104ABC File Offset: 0x00102CBC
	public static bool IsHidden
	{
		get
		{
			return HudGlobalHide._isHidden;
		}
		set
		{
			HudGlobalHide._isHidden = value;
			foreach (HudGlobalHide hudGlobalHide in HudGlobalHide._activeObjs)
			{
				hudGlobalHide.UpdateLocation();
			}
		}
	}

	// Token: 0x170006AF RID: 1711
	// (get) Token: 0x06003B2C RID: 15148 RVA: 0x00104B14 File Offset: 0x00102D14
	// (set) Token: 0x06003B2D RID: 15149 RVA: 0x00104B1C File Offset: 0x00102D1C
	public static bool IsReduced
	{
		get
		{
			return HudGlobalHide._isReduced;
		}
		set
		{
			HudGlobalHide._isReduced = value;
			foreach (HudGlobalHide hudGlobalHide in HudGlobalHide._activeObjs)
			{
				hudGlobalHide.UpdateLocation();
			}
		}
	}

	// Token: 0x06003B2E RID: 15150 RVA: 0x00104B74 File Offset: 0x00102D74
	private void OnEnable()
	{
		HudGlobalHide._activeObjs.Add(this);
		this.UpdateLocation();
	}

	// Token: 0x06003B2F RID: 15151 RVA: 0x00104B88 File Offset: 0x00102D88
	private void OnDisable()
	{
		HudGlobalHide._activeObjs.Remove(this);
		if (HudGlobalHide._activeObjs.Count == 0)
		{
			HudGlobalHide._isHidden = false;
		}
	}

	// Token: 0x06003B30 RID: 15152 RVA: 0x00104BA8 File Offset: 0x00102DA8
	private void UpdateLocation()
	{
		Transform transform = base.transform;
		if (HudGlobalHide._isHidden)
		{
			transform.localPosition = new Vector3(0f, -200f, 0f);
			return;
		}
		if (HudGlobalHide._isReduced)
		{
			transform.localPosition = this.reducedPos;
			transform.localScale = this.reducedScale.ToVector3(1f);
			return;
		}
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	// Token: 0x04003D75 RID: 15733
	[SerializeField]
	private Vector2 reducedPos;

	// Token: 0x04003D76 RID: 15734
	[SerializeField]
	private Vector2 reducedScale;

	// Token: 0x04003D77 RID: 15735
	private static readonly HashSet<HudGlobalHide> _activeObjs = new HashSet<HudGlobalHide>();

	// Token: 0x04003D78 RID: 15736
	private static bool _isHidden;

	// Token: 0x04003D79 RID: 15737
	private static bool _isReduced;
}
