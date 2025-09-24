using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class CameraShakeOnEnable : MonoBehaviour
{
	// Token: 0x06001451 RID: 5201 RVA: 0x0005B628 File Offset: 0x00059828
	private void OnEnable()
	{
		this.scheduleRoutine = base.StartCoroutine(this.ShakeCameraDelayed());
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x0005B63C File Offset: 0x0005983C
	private void OnDisable()
	{
		if (this.scheduleRoutine != null)
		{
			base.StopCoroutine(this.scheduleRoutine);
			this.scheduleRoutine = null;
		}
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x0005B659 File Offset: 0x00059859
	private IEnumerator ShakeCameraDelayed()
	{
		yield return new WaitForEndOfFrame();
		if (this.delay > 0f)
		{
			yield return new WaitForSeconds(this.delay);
		}
		this.CameraShake.DoShake(this, true);
		yield break;
	}

	// Token: 0x0400128F RID: 4751
	public CameraShakeTarget CameraShake;

	// Token: 0x04001290 RID: 4752
	public float delay;

	// Token: 0x04001291 RID: 4753
	private Coroutine scheduleRoutine;
}
