using System;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000794 RID: 1940
public class ToolBreakRangeHandler : MonoBehaviour
{
	// Token: 0x0600449F RID: 17567 RVA: 0x0012C730 File Offset: 0x0012A930
	private void OnEnable()
	{
		this.camera = GameCameras.instance.mainCamera.transform;
		this.maxRange = Gameplay.ToolCameraDistanceBreak;
		this.graceTimeLeft = 0.1f;
		if (this.wasOutsideRange)
		{
			this.OnEnterRange.Invoke();
			this.wasOutsideRange = false;
		}
	}

	// Token: 0x060044A0 RID: 17568 RVA: 0x0012C784 File Offset: 0x0012A984
	private void Update()
	{
		if (this.graceTimeLeft > 0f)
		{
			this.graceTimeLeft -= Time.deltaTime;
			return;
		}
		Vector2 a = this.camera.position;
		Vector2 b = base.transform.position;
		bool flag = Vector2.Distance(a, b) >= this.maxRange;
		if (flag)
		{
			if (!this.wasOutsideRange)
			{
				this.OnExitRange.Invoke();
			}
			this.OnOutsideRange.Invoke();
		}
		else
		{
			if (this.wasOutsideRange)
			{
				this.OnEnterRange.Invoke();
			}
			this.OnInsideRange.Invoke();
		}
		this.wasOutsideRange = flag;
	}

	// Token: 0x04004598 RID: 17816
	private const float ENABLE_GRACE_TIME = 0.1f;

	// Token: 0x04004599 RID: 17817
	public UnityEvent OnExitRange;

	// Token: 0x0400459A RID: 17818
	public UnityEvent OnOutsideRange;

	// Token: 0x0400459B RID: 17819
	public UnityEvent OnEnterRange;

	// Token: 0x0400459C RID: 17820
	public UnityEvent OnInsideRange;

	// Token: 0x0400459D RID: 17821
	private Transform camera;

	// Token: 0x0400459E RID: 17822
	private float maxRange;

	// Token: 0x0400459F RID: 17823
	private bool wasOutsideRange;

	// Token: 0x040045A0 RID: 17824
	private float graceTimeLeft;
}
