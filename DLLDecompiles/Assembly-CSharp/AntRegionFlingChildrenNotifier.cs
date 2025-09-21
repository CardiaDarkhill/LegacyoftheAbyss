using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074D RID: 1869
public sealed class AntRegionFlingChildrenNotifier : MonoBehaviour, AntRegion.IPickUpNotify
{
	// Token: 0x0600427A RID: 17018 RVA: 0x001255A0 File Offset: 0x001237A0
	private void Awake()
	{
		if (!this.flingChildrenOnStart)
		{
			this.flingChildrenOnStart = base.GetComponent<FlingChildrenOnStart>();
			if (!this.flingChildrenOnStart)
			{
				return;
			}
		}
		foreach (object obj in base.transform)
		{
			AntRegionFlingChildrenNotifier antRegionFlingChildrenNotifier = ((Transform)obj).gameObject.AddComponentIfNotPresent<AntRegionFlingChildrenNotifier>();
			this.childNotifiers.Add(antRegionFlingChildrenNotifier);
			antRegionFlingChildrenNotifier.parentNotifier = this;
		}
	}

	// Token: 0x0600427B RID: 17019 RVA: 0x00125638 File Offset: 0x00123838
	private void OnValidate()
	{
		if (!this.flingChildrenOnStart)
		{
			this.flingChildrenOnStart = base.GetComponent<FlingChildrenOnStart>();
		}
	}

	// Token: 0x0600427C RID: 17020 RVA: 0x00125654 File Offset: 0x00123854
	private void OnDisable()
	{
		if (this.activeRegions.Count > 0)
		{
			this.activeRegions.Clear();
			if (this.parentNotifier)
			{
				this.parentNotifier.ChildExitedAntRegion();
				return;
			}
			if (this.resetDynamicHierarchy)
			{
				this.resetDynamicHierarchy.Reconnect(base.transform, true, true);
			}
		}
	}

	// Token: 0x0600427D RID: 17021 RVA: 0x001256B4 File Offset: 0x001238B4
	private void EnteredAntRegion()
	{
		if (this.disconnected)
		{
			return;
		}
		if (!this.resetDynamicHierarchy)
		{
			if (!this.flingChildrenOnStart)
			{
				return;
			}
			Transform parentParent = this.flingChildrenOnStart.GetParentParent();
			if (!parentParent)
			{
				return;
			}
			this.resetDynamicHierarchy = parentParent.GetComponent<ResetDynamicHierarchy>();
			if (!this.resetDynamicHierarchy)
			{
				return;
			}
		}
		if (this.childNotifiers.Count == 0)
		{
			return;
		}
		bool flag = true;
		using (List<AntRegionFlingChildrenNotifier>.Enumerator enumerator = this.childNotifiers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.activeRegions.Count > 0)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			return;
		}
		this.resetDynamicHierarchy.Disconnect(base.transform, true);
		this.disconnected = true;
	}

	// Token: 0x0600427E RID: 17022 RVA: 0x00125790 File Offset: 0x00123990
	private void ChildExitedAntRegion()
	{
		if (!this.disconnected)
		{
			return;
		}
		using (List<AntRegionFlingChildrenNotifier>.Enumerator enumerator = this.childNotifiers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.activeRegions.Count > 0)
				{
					return;
				}
			}
		}
		if (this.resetDynamicHierarchy)
		{
			this.resetDynamicHierarchy.Reconnect(base.transform, true, true);
		}
		this.disconnected = false;
	}

	// Token: 0x0600427F RID: 17023 RVA: 0x0012581C File Offset: 0x00123A1C
	public void PickUpStarted(AntRegion antRegion)
	{
		if (antRegion == null)
		{
			return;
		}
		if (!this.activeRegions.Add(antRegion))
		{
			return;
		}
		if (this.parentNotifier)
		{
			this.parentNotifier.EnteredAntRegion();
			return;
		}
		this.EnteredAntRegion();
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x00125856 File Offset: 0x00123A56
	public void PickUpEnd(AntRegion antRegion)
	{
		if (!this.activeRegions.Remove(antRegion))
		{
			return;
		}
		if (this.parentNotifier)
		{
			this.parentNotifier.ChildExitedAntRegion();
			return;
		}
		this.ChildExitedAntRegion();
	}

	// Token: 0x0400440B RID: 17419
	[SerializeField]
	private FlingChildrenOnStart flingChildrenOnStart;

	// Token: 0x0400440C RID: 17420
	private ResetDynamicHierarchy resetDynamicHierarchy;

	// Token: 0x0400440D RID: 17421
	private AntRegionFlingChildrenNotifier parentNotifier;

	// Token: 0x0400440E RID: 17422
	private List<AntRegionFlingChildrenNotifier> childNotifiers = new List<AntRegionFlingChildrenNotifier>();

	// Token: 0x0400440F RID: 17423
	private HashSet<AntRegion> activeRegions = new HashSet<AntRegion>();

	// Token: 0x04004410 RID: 17424
	private bool disconnected;
}
