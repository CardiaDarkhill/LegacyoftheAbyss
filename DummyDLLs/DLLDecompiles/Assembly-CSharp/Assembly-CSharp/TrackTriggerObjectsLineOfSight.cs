using System;
using UnityEngine;

// Token: 0x020005FA RID: 1530
public class TrackTriggerObjectsLineOfSight : TrackTriggerObjects
{
	// Token: 0x060036A0 RID: 13984 RVA: 0x000F0FF8 File Offset: 0x000EF1F8
	protected override bool IsCounted(GameObject obj)
	{
		Transform transform = base.transform;
		TrackTriggerObjectsLineOfSight.LineOfSightChecks lineOfSightChecks = this.lineOfSightCheck;
		Transform transform2;
		if (lineOfSightChecks != TrackTriggerObjectsLineOfSight.LineOfSightChecks.Self)
		{
			if (lineOfSightChecks != TrackTriggerObjectsLineOfSight.LineOfSightChecks.Parent)
			{
				throw new NotImplementedException();
			}
			Transform parent = transform.parent;
			transform2 = (parent ? parent : transform);
		}
		else
		{
			transform2 = transform;
		}
		if (!transform2)
		{
			return false;
		}
		Vector2 vector = transform2.position;
		Vector2 vector2 = obj.transform.position - vector;
		return !Helper.IsRayHittingNoTriggers(vector, vector2.normalized, vector2.magnitude, 256);
	}

	// Token: 0x0400396D RID: 14701
	[SerializeField]
	private TrackTriggerObjectsLineOfSight.LineOfSightChecks lineOfSightCheck;

	// Token: 0x02001901 RID: 6401
	private enum LineOfSightChecks
	{
		// Token: 0x0400941A RID: 37914
		Self,
		// Token: 0x0400941B RID: 37915
		Parent
	}
}
