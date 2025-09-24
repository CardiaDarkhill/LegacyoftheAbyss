using System;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x020007BC RID: 1980
[DisallowMultipleComponent]
public class LiftChain : MonoBehaviour
{
	// Token: 0x060045CC RID: 17868 RVA: 0x0012F9FB File Offset: 0x0012DBFB
	protected void Awake()
	{
		this.spline.UpdateCondition = SplineBase.UpdateConditions.Manual;
		this.spline.SetDynamic();
	}

	// Token: 0x060045CD RID: 17869 RVA: 0x0012FA14 File Offset: 0x0012DC14
	private void Update()
	{
		if (!this.isMoving)
		{
			return;
		}
		this.spline.TextureOffset += this.speed * this.moveDir * Time.deltaTime;
		if (this.fpsLimit > Mathf.Epsilon)
		{
			if (Time.timeAsDouble < this.nextUpdateTime)
			{
				return;
			}
			this.nextUpdateTime = Time.timeAsDouble + (double)(1f / this.fpsLimit);
		}
		this.spline.UpdateSpline();
	}

	// Token: 0x060045CE RID: 17870 RVA: 0x0012FA8E File Offset: 0x0012DC8E
	public void GoDown()
	{
		this.isMoving = true;
		this.moveDir = 1f;
	}

	// Token: 0x060045CF RID: 17871 RVA: 0x0012FAA2 File Offset: 0x0012DCA2
	public void GoUp()
	{
		this.isMoving = true;
		this.moveDir = -1f;
	}

	// Token: 0x060045D0 RID: 17872 RVA: 0x0012FAB6 File Offset: 0x0012DCB6
	public void Stop()
	{
		this.isMoving = false;
	}

	// Token: 0x04004679 RID: 18041
	[SerializeField]
	private SplineBase spline;

	// Token: 0x0400467A RID: 18042
	[SerializeField]
	private float speed;

	// Token: 0x0400467B RID: 18043
	[SerializeField]
	private float fpsLimit;

	// Token: 0x0400467C RID: 18044
	private bool isMoving;

	// Token: 0x0400467D RID: 18045
	private float moveDir;

	// Token: 0x0400467E RID: 18046
	private double nextUpdateTime;
}
