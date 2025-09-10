using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000449 RID: 1097
public class ParentAngleEvent : EventBase
{
	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x0600269D RID: 9885 RVA: 0x000AEC2E File Offset: 0x000ACE2E
	public override string InspectorInfo
	{
		get
		{
			return string.Format("From parent \"{0}\"", this.parent ? this.parent.name : "null");
		}
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000AEC5C File Offset: 0x000ACE5C
	private void Update()
	{
		Vector2 vector = base.transform.position - this.parent.position;
		float value = Vector2.Angle(Vector2.right, vector.normalized);
		bool flag = this.angleRange.IsInRange(value);
		if (!this.wasInsideRange && flag)
		{
			base.CallReceivedEvent();
		}
		this.wasInsideRange = flag;
	}

	// Token: 0x040023FC RID: 9212
	[SerializeField]
	private Transform parent;

	// Token: 0x040023FD RID: 9213
	[SerializeField]
	private MinMaxFloat angleRange;

	// Token: 0x040023FE RID: 9214
	private bool wasInsideRange;
}
