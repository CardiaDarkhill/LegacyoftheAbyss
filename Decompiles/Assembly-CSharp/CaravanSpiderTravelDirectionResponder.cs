using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class CaravanSpiderTravelDirectionResponder : MonoBehaviour
{
	// Token: 0x06000BD3 RID: 3027 RVA: 0x00035B70 File Offset: 0x00033D70
	private void OnEnable()
	{
		if (!this.animator)
		{
			return;
		}
		float caravanSpiderTravelDirection = PlayerData.instance.CaravanSpiderTravelDirection;
		float x = this.speed * Mathf.Cos(caravanSpiderTravelDirection * 0.017453292f);
		float y = this.speed * Mathf.Sin(caravanSpiderTravelDirection * 0.017453292f);
		this.animator.Offset = new Vector3(x, y, 0f);
	}

	// Token: 0x04000B5F RID: 2911
	[SerializeField]
	private CurveOffsetAnimation animator;

	// Token: 0x04000B60 RID: 2912
	[SerializeField]
	private float speed;
}
