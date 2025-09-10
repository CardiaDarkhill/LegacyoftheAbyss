using System;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class LoopMovementTrack : MonoBehaviour
{
	// Token: 0x060004AA RID: 1194 RVA: 0x000190B4 File Offset: 0x000172B4
	private void OnValidate()
	{
		if (this.childrenToMove != null)
		{
			for (int i = 0; i < this.childrenToMove.Length; i++)
			{
				if (this.childrenToMove[i].parent != base.transform)
				{
					this.childrenToMove[i] = null;
					Debug.LogError("Assigned transform must be a child of this transform!", this);
				}
			}
		}
		if (this.loopTime < 0f)
		{
			this.loopTime = 0f;
		}
		this.UpdateChildren(0f);
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001912D File Offset: 0x0001732D
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.startPos, 0.2f);
		Gizmos.DrawWireSphere(this.endPos, 0.2f);
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00019160 File Offset: 0x00017360
	private void Update()
	{
		if (this.loopTime <= 0f)
		{
			return;
		}
		float timeOffset = Time.time / this.loopTime;
		this.UpdateChildren(timeOffset);
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00019190 File Offset: 0x00017390
	private void UpdateChildren(float timeOffset)
	{
		if (this.childrenToMove == null || this.childrenToMove.Length == 0)
		{
			return;
		}
		int num = this.childrenToMove.Length;
		int num2 = num - 1;
		for (int i = 0; i < num; i++)
		{
			Transform transform = this.childrenToMove[i];
			float num3 = (float)i / (float)num2;
			float num4 = (timeOffset + num3) % 1f;
			if (this.isReversed)
			{
				num4 = 1f - num4;
			}
			transform.localPosition = Vector3.Lerp(this.startPos, this.endPos, this.movementCurve.Evaluate(num4));
			transform.localScale = Vector3.Lerp(this.startScale, this.endScale, this.scaleCurve.Evaluate(num4));
		}
	}

	// Token: 0x04000472 RID: 1138
	[SerializeField]
	private Transform[] childrenToMove;

	// Token: 0x04000473 RID: 1139
	[Space]
	[SerializeField]
	private Vector3 startPos;

	// Token: 0x04000474 RID: 1140
	[SerializeField]
	private Vector3 endPos;

	// Token: 0x04000475 RID: 1141
	[Space]
	[SerializeField]
	private Vector3 startScale = Vector3.one;

	// Token: 0x04000476 RID: 1142
	[SerializeField]
	private Vector3 endScale = Vector3.one;

	// Token: 0x04000477 RID: 1143
	[Space]
	[SerializeField]
	private float loopTime;

	// Token: 0x04000478 RID: 1144
	[SerializeField]
	private AnimationCurve movementCurve;

	// Token: 0x04000479 RID: 1145
	[SerializeField]
	private AnimationCurve scaleCurve;

	// Token: 0x0400047A RID: 1146
	[SerializeField]
	private bool isReversed;
}
