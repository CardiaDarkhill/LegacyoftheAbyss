using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000087 RID: 135
public class FloatEventSpeed : MonoBehaviour
{
	// Token: 0x060003C9 RID: 969 RVA: 0x00012E0C File Offset: 0x0001100C
	private void LateUpdate()
	{
		if (Math.Abs(this.speed) <= Mathf.Epsilon && Math.Abs(this.previousSpeed) <= Mathf.Epsilon)
		{
			return;
		}
		this.SpeedEvent.Invoke(this.speed);
		this.total += this.speed * Time.deltaTime;
		this.TotalEvent.Invoke(this.total);
	}

	// Token: 0x04000365 RID: 869
	[SerializeField]
	private float speed;

	// Token: 0x04000366 RID: 870
	public UnityEvent<float> SpeedEvent;

	// Token: 0x04000367 RID: 871
	public UnityEvent<float> TotalEvent;

	// Token: 0x04000368 RID: 872
	private float total;

	// Token: 0x04000369 RID: 873
	private float previousSpeed;
}
