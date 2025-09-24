using System;
using UnityEngine;

// Token: 0x0200052F RID: 1327
public class PointAtPivot : MonoBehaviour
{
	// Token: 0x06002F9E RID: 12190 RVA: 0x000D1B69 File Offset: 0x000CFD69
	private void Awake()
	{
		if (this.pivot == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x000D1B80 File Offset: 0x000CFD80
	private void Start()
	{
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x000D1B84 File Offset: 0x000CFD84
	private void LateUpdate()
	{
		Transform transform = base.transform;
		Vector3 position = this.pivot.position;
		Vector3 position2 = transform.position;
		float y = position.y - position2.y;
		float x = position.x - position2.x;
		float num;
		for (num = Mathf.Atan2(y, x) * 57.295776f + this.angleOffset; num < 0f; num += 360f)
		{
		}
		transform.eulerAngles = new Vector3(0f, 0f, num);
	}

	// Token: 0x04003261 RID: 12897
	[SerializeField]
	private Transform pivot;

	// Token: 0x04003262 RID: 12898
	[SerializeField]
	private float angleOffset;
}
