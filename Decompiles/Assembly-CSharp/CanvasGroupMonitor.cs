using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000610 RID: 1552
public class CanvasGroupMonitor : MonoBehaviour
{
	// Token: 0x17000659 RID: 1625
	// (get) Token: 0x0600376B RID: 14187 RVA: 0x000F47B4 File Offset: 0x000F29B4
	// (set) Token: 0x0600376C RID: 14188 RVA: 0x000F47BC File Offset: 0x000F29BC
	public CanvasGroup Group
	{
		get
		{
			return this.group;
		}
		set
		{
			this.group = value;
		}
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x000F47C5 File Offset: 0x000F29C5
	private void OnEnable()
	{
		this.previousAlpha = this.group.alpha;
		this.OnAlphaChanged.Invoke(this.previousAlpha);
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x000F47EC File Offset: 0x000F29EC
	private void Update()
	{
		if (!this.group || this.OnAlphaChanged == null)
		{
			return;
		}
		if (Math.Abs(this.group.alpha - this.previousAlpha) > Mathf.Epsilon)
		{
			this.previousAlpha = this.group.alpha;
			this.OnAlphaChanged.Invoke(this.previousAlpha);
		}
	}

	// Token: 0x04003A57 RID: 14935
	[SerializeField]
	private CanvasGroup group;

	// Token: 0x04003A58 RID: 14936
	[Space]
	public CanvasGroupMonitor.UnityEventFloat OnAlphaChanged;

	// Token: 0x04003A59 RID: 14937
	private float previousAlpha = -1f;

	// Token: 0x02001925 RID: 6437
	[Serializable]
	public class UnityEventFloat : UnityEvent<float>
	{
	}
}
