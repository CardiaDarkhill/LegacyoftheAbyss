using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200064C RID: 1612
[ExecuteInEditMode]
[NestedFadeGroupBridge(new Type[]
{
	typeof(GradeMarker)
})]
[RequireComponent(typeof(GradeMarker))]
public class NestedFadeGroupGradeMarker : NestedFadeGroupBase
{
	// Token: 0x060039B9 RID: 14777 RVA: 0x000FD462 File Offset: 0x000FB662
	protected override void GetMissingReferences()
	{
		if (!this.gradeMarker)
		{
			this.gradeMarker = base.GetComponent<GradeMarker>();
		}
	}

	// Token: 0x060039BA RID: 14778 RVA: 0x000FD47D File Offset: 0x000FB67D
	protected override void OnAlphaChanged(float alpha)
	{
		this.gradeMarker.Alpha = alpha;
	}

	// Token: 0x04003C6D RID: 15469
	private GradeMarker gradeMarker;
}
