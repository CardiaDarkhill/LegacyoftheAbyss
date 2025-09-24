using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class ChildWheelAnimation : MonoBehaviour
{
	// Token: 0x060002FE RID: 766 RVA: 0x000101ED File Offset: 0x0000E3ED
	private void Reset()
	{
		this.rotate = base.transform;
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000101FC File Offset: 0x0000E3FC
	private void OnEnable()
	{
		if (!this.trackPosition || !this.rotate)
		{
			base.enabled = false;
			return;
		}
		this.previousXPos = this.trackPosition.localPosition.x;
		this.sign = Mathf.Sign(this.trackPosition.lossyScale.x);
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0001025C File Offset: 0x0000E45C
	private void LateUpdate()
	{
		float x = this.trackPosition.localPosition.x;
		float num = x - this.previousXPos;
		if (Math.Abs(num) < 0.001f)
		{
			return;
		}
		this.rotate.Rotate(new Vector3(0f, 0f, num * this.rotateAmount * this.sign));
		this.previousXPos = x;
	}

	// Token: 0x04000285 RID: 645
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform trackPosition;

	// Token: 0x04000286 RID: 646
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform rotate;

	// Token: 0x04000287 RID: 647
	[SerializeField]
	private float rotateAmount;

	// Token: 0x04000288 RID: 648
	private float previousXPos;

	// Token: 0x04000289 RID: 649
	private float sign;
}
