using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class RandomRotationRange : MonoBehaviour
{
	// Token: 0x0600161C RID: 5660 RVA: 0x000630EA File Offset: 0x000612EA
	private void Start()
	{
		this.RandomRotate();
		this.started = true;
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x000630F9 File Offset: 0x000612F9
	private void OnEnable()
	{
		if (this.started)
		{
			this.RandomRotate();
		}
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x0006310C File Offset: 0x0006130C
	private void RandomRotate()
	{
		if (this.relativeToStartRotation)
		{
			float num = Random.Range(this.min, this.max);
			float z = base.transform.localEulerAngles.z + num;
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, z);
			return;
		}
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, Random.Range(this.min, this.max));
	}

	// Token: 0x04001484 RID: 5252
	public float min;

	// Token: 0x04001485 RID: 5253
	public float max;

	// Token: 0x04001486 RID: 5254
	public bool relativeToStartRotation;

	// Token: 0x04001487 RID: 5255
	private bool started;
}
