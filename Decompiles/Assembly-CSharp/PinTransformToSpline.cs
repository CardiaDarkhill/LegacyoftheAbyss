using System;
using JetBrains.Annotations;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x0200052C RID: 1324
[ExecuteInEditMode]
public class PinTransformToSpline : MonoBehaviour
{
	// Token: 0x17000545 RID: 1349
	// (get) Token: 0x06002F8E RID: 12174 RVA: 0x000D164C File Offset: 0x000CF84C
	// (set) Token: 0x06002F8F RID: 12175 RVA: 0x000D1654 File Offset: 0x000CF854
	public SplineBase Spline
	{
		get
		{
			return this.spline;
		}
		set
		{
			this.spline = value;
		}
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x000D165D File Offset: 0x000CF85D
	[UsedImplicitly]
	private bool IsRotate()
	{
		return this.rotateBehaviour > PinTransformToSpline.RotateBehaviours.None;
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x000D1668 File Offset: 0x000CF868
	[UsedImplicitly]
	private bool IsRotateAlong()
	{
		return this.rotateBehaviour == PinTransformToSpline.RotateBehaviours.RotateAlong;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x000D1674 File Offset: 0x000CF874
	private void LateUpdate()
	{
		if (!this.spline)
		{
			return;
		}
		int pointCount = this.spline.GetPointCount();
		if (pointCount < 1)
		{
			return;
		}
		float num = (float)pointCount * this.splinePosition;
		int index = Mathf.Clamp(Mathf.FloorToInt(num), 0, pointCount - 1);
		int index2 = Mathf.Clamp(Mathf.CeilToInt(num), 0, pointCount - 1);
		float t = num % 1f;
		SplineBase.Point point = this.spline.GetPoint(index);
		SplineBase.Point point2 = this.spline.GetPoint(index2);
		Transform transform = this.spline.transform;
		Vector3 position = Vector3.Lerp(point.Position, point2.Position, t);
		Vector3 vector = Vector3.Lerp(point.Tangent, point2.Tangent, t);
		vector = transform.TransformVector(vector).normalized;
		base.transform.SetPosition2D(transform.TransformPoint(position));
		Quaternion rhs;
		switch (this.rotateBehaviour)
		{
		case PinTransformToSpline.RotateBehaviours.None:
			return;
		case PinTransformToSpline.RotateBehaviours.RotateWith:
			rhs = Quaternion.LookRotation(Vector3.forward, -vector);
			break;
		case PinTransformToSpline.RotateBehaviours.RotateAlong:
		{
			float length = this.spline.Length;
			if (Math.Abs(length) <= Mathf.Epsilon)
			{
				return;
			}
			rhs = Quaternion.Euler(0f, 0f, this.rotateAmount * this.splinePosition * length);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		Quaternion quaternion = transform.rotation * rhs;
		if (Math.Abs(this.rotationOffset) > Mathf.Epsilon)
		{
			quaternion *= Quaternion.Euler(0f, 0f, this.rotationOffset);
		}
		base.transform.rotation = quaternion;
	}

	// Token: 0x0400324F RID: 12879
	[SerializeField]
	private SplineBase spline;

	// Token: 0x04003250 RID: 12880
	[SerializeField]
	[Range(0f, 1f)]
	private float splinePosition;

	// Token: 0x04003251 RID: 12881
	[SerializeField]
	private PinTransformToSpline.RotateBehaviours rotateBehaviour = PinTransformToSpline.RotateBehaviours.RotateWith;

	// Token: 0x04003252 RID: 12882
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsRotateAlong", true, true, true)]
	private float rotateAmount;

	// Token: 0x04003253 RID: 12883
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsRotate", true, true, true)]
	private float rotationOffset;

	// Token: 0x0200183E RID: 6206
	private enum RotateBehaviours
	{
		// Token: 0x04009142 RID: 37186
		None,
		// Token: 0x04009143 RID: 37187
		RotateWith,
		// Token: 0x04009144 RID: 37188
		RotateAlong
	}
}
