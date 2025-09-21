using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public sealed class SandRegionCollisionBreakable : MonoBehaviour
{
	// Token: 0x06000723 RID: 1827 RVA: 0x00023590 File Offset: 0x00021790
	private bool? IsFsmEventValidRequired(string eventName)
	{
		return this.fsm.IsEventValid(eventName, true);
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x0002359F File Offset: 0x0002179F
	private void OnEnable()
	{
		this.isBroken = false;
		if (this.fsm == null)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x000235BC File Offset: 0x000217BC
	private void OnDisable()
	{
		this.sandRegions.Clear();
		this.isInsideSandRegion = false;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x000235D0 File Offset: 0x000217D0
	public void AddSandRegion(SandRegion sandRegion)
	{
		this.sandRegions.Add(sandRegion);
		this.isInsideSandRegion = true;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x000235E6 File Offset: 0x000217E6
	public void RemoveSandRegion(SandRegion sandRegion)
	{
		if (this.sandRegions.Remove(sandRegion) && this.sandRegions.Count == 0)
		{
			this.isInsideSandRegion = false;
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0002360A File Offset: 0x0002180A
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (this.isBroken)
		{
			return;
		}
		if (!this.isInsideSandRegion)
		{
			return;
		}
		if (other.gameObject.layer == 8)
		{
			this.Break();
		}
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00023632 File Offset: 0x00021832
	private void Break()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent(this.breakEvent);
		}
	}

	// Token: 0x040006EE RID: 1774
	[SerializeField]
	private PlayMakerFSM fsm;

	// Token: 0x040006EF RID: 1775
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string breakEvent = "BREAK";

	// Token: 0x040006F0 RID: 1776
	private HashSet<SandRegion> sandRegions = new HashSet<SandRegion>();

	// Token: 0x040006F1 RID: 1777
	private bool isInsideSandRegion;

	// Token: 0x040006F2 RID: 1778
	private bool isBroken;
}
