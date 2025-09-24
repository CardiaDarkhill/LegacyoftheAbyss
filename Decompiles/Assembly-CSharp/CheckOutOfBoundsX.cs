using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000179 RID: 377
public class CheckOutOfBoundsX : MonoBehaviour
{
	// Token: 0x06000C57 RID: 3159 RVA: 0x00036325 File Offset: 0x00034525
	private void OnEnable()
	{
		this.fired = false;
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00036330 File Offset: 0x00034530
	private void Update()
	{
		if (!this.fired)
		{
			float x = base.transform.position.x;
			if (x < this.xMin || x > this.xMax)
			{
				this.onOutOfBounds.Invoke();
				this.fired = true;
			}
		}
	}

	// Token: 0x04000BCD RID: 3021
	public float xMin;

	// Token: 0x04000BCE RID: 3022
	public float xMax;

	// Token: 0x04000BCF RID: 3023
	public UnityEvent onOutOfBounds;

	// Token: 0x04000BD0 RID: 3024
	private bool fired;
}
