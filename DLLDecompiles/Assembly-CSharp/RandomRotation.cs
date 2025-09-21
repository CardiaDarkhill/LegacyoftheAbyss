using System;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class RandomRotation : MonoBehaviour
{
	// Token: 0x06001614 RID: 5652 RVA: 0x0006305B File Offset: 0x0006125B
	private void Start()
	{
		this.RandomRotate();
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x00063063 File Offset: 0x00061263
	private void OnEnable()
	{
		this.RandomRotate();
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x0006306C File Offset: 0x0006126C
	private void RandomRotate()
	{
		Transform transform = base.transform;
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles.z = Random.Range(0f, 360f);
		transform.localEulerAngles = localEulerAngles;
	}
}
