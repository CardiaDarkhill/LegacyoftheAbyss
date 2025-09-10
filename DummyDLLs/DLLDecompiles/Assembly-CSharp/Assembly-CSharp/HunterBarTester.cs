using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200076E RID: 1902
public sealed class HunterBarTester : MonoBehaviour
{
	// Token: 0x17000798 RID: 1944
	// (get) Token: 0x060043DF RID: 17375 RVA: 0x0012A0D4 File Offset: 0x001282D4
	// (set) Token: 0x060043E0 RID: 17376 RVA: 0x0012A0DC File Offset: 0x001282DC
	public float Progress
	{
		get
		{
			return this.progress;
		}
		set
		{
			this.progress = value;
			foreach (UiProgressBar uiProgressBar in this.progressBars)
			{
				uiProgressBar.Value = this.progress;
			}
		}
	}

	// Token: 0x17000799 RID: 1945
	// (get) Token: 0x060043E1 RID: 17377 RVA: 0x0012A13C File Offset: 0x0012833C
	// (set) Token: 0x060043E2 RID: 17378 RVA: 0x0012A144 File Offset: 0x00128344
	public float Angle
	{
		get
		{
			return this.angle;
		}
		set
		{
			this.angle = value;
			foreach (UiProgressBar uiProgressBar in this.progressBars)
			{
				uiProgressBar.SetAngle(value);
			}
		}
	}

	// Token: 0x1700079A RID: 1946
	// (get) Token: 0x060043E3 RID: 17379 RVA: 0x0012A19C File Offset: 0x0012839C
	// (set) Token: 0x060043E4 RID: 17380 RVA: 0x0012A1A4 File Offset: 0x001283A4
	public float EdgeFade
	{
		get
		{
			return this.edgeFade;
		}
		set
		{
			this.edgeFade = value;
			foreach (UiProgressBar uiProgressBar in this.progressBars)
			{
				uiProgressBar.SetEdgeFade(value);
			}
		}
	}

	// Token: 0x1700079B RID: 1947
	// (get) Token: 0x060043E5 RID: 17381 RVA: 0x0012A1FC File Offset: 0x001283FC
	// (set) Token: 0x060043E6 RID: 17382 RVA: 0x0012A204 File Offset: 0x00128404
	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
		set
		{
			this.isActive = value;
			foreach (UiProgressBar uiProgressBar in this.progressBars)
			{
				uiProgressBar.gameObject.SetActive(value);
			}
		}
	}

	// Token: 0x060043E7 RID: 17383 RVA: 0x0012A264 File Offset: 0x00128464
	private void Awake()
	{
		this.progressBars.Clear();
		this.progressBars.AddRange(base.GetComponentsInChildren<UiProgressBar>());
		this.Progress = 1f;
		this.IsActive = false;
	}

	// Token: 0x060043E8 RID: 17384 RVA: 0x0012A294 File Offset: 0x00128494
	public void UpdateBar(float deltaTime)
	{
		foreach (UiProgressBar uiProgressBar in this.progressBars)
		{
			uiProgressBar.UpdateBar(deltaTime);
		}
	}

	// Token: 0x04004531 RID: 17713
	[SerializeField]
	private List<UiProgressBar> progressBars = new List<UiProgressBar>();

	// Token: 0x04004532 RID: 17714
	private float progress = 1f;

	// Token: 0x04004533 RID: 17715
	private float angle;

	// Token: 0x04004534 RID: 17716
	private float edgeFade = 3f;

	// Token: 0x04004535 RID: 17717
	private bool isActive;
}
