using System;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x020004F5 RID: 1269
[ExecuteInEditMode]
public class HarpoonRingPoleConfigure : MonoBehaviour
{
	// Token: 0x06002D71 RID: 11633 RVA: 0x000C64EC File Offset: 0x000C46EC
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002D72 RID: 11634 RVA: 0x000C64F4 File Offset: 0x000C46F4
	private void Setup()
	{
		if (!this.ring || !this.pole)
		{
			return;
		}
		float x = this.ring.localPosition.x;
		Transform endPoint = this.pole.EndPoint;
		Vector3 localPosition = endPoint.localPosition;
		localPosition.x = x + this.poleTopOffset;
		if (Math.Abs(endPoint.localPosition.x - localPosition.x) > Mathf.Epsilon)
		{
			endPoint.localPosition = localPosition;
		}
		Transform controlPoint = this.pole.ControlPoint;
		Vector3 localPosition2 = controlPoint.localPosition;
		localPosition2.x = localPosition.x * this.controlPointLoc;
		if (Math.Abs(controlPoint.localPosition.x - localPosition2.x) > Mathf.Epsilon)
		{
			controlPoint.localPosition = localPosition2;
		}
		float num = this.textureTiling * localPosition.x;
		if (Math.Abs(this.pole.TextureTiling - num) > Mathf.Epsilon)
		{
			this.pole.TextureTiling = num;
		}
	}

	// Token: 0x04002F21 RID: 12065
	[SerializeField]
	private Transform ring;

	// Token: 0x04002F22 RID: 12066
	[SerializeField]
	private QuadraticBezierSpline pole;

	// Token: 0x04002F23 RID: 12067
	[SerializeField]
	private float poleTopOffset;

	// Token: 0x04002F24 RID: 12068
	[SerializeField]
	[Range(0f, 1f)]
	private float controlPointLoc;

	// Token: 0x04002F25 RID: 12069
	[SerializeField]
	private float textureTiling;
}
