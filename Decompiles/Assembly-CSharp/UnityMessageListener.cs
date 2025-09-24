using System;
using UnityEngine;

// Token: 0x02000747 RID: 1863
[ExecuteInEditMode]
public class UnityMessageListener : MonoBehaviour
{
	// Token: 0x140000E0 RID: 224
	// (add) Token: 0x06004262 RID: 16994 RVA: 0x00125268 File Offset: 0x00123468
	// (remove) Token: 0x06004263 RID: 16995 RVA: 0x001252A0 File Offset: 0x001234A0
	public event Action Enabled;

	// Token: 0x140000E1 RID: 225
	// (add) Token: 0x06004264 RID: 16996 RVA: 0x001252D8 File Offset: 0x001234D8
	// (remove) Token: 0x06004265 RID: 16997 RVA: 0x00125310 File Offset: 0x00123510
	public event Action Disabled;

	// Token: 0x140000E2 RID: 226
	// (add) Token: 0x06004266 RID: 16998 RVA: 0x00125348 File Offset: 0x00123548
	// (remove) Token: 0x06004267 RID: 16999 RVA: 0x00125380 File Offset: 0x00123580
	public event Action TransformParentChanged;

	// Token: 0x17000785 RID: 1925
	// (get) Token: 0x06004268 RID: 17000 RVA: 0x001253B5 File Offset: 0x001235B5
	private bool CanExecute
	{
		get
		{
			return Application.isPlaying || this.ExecuteInEditMode;
		}
	}

	// Token: 0x06004269 RID: 17001 RVA: 0x001253C6 File Offset: 0x001235C6
	private void OnEnable()
	{
		if (!this.CanExecute)
		{
			return;
		}
		Action enabled = this.Enabled;
		if (enabled == null)
		{
			return;
		}
		enabled();
	}

	// Token: 0x0600426A RID: 17002 RVA: 0x001253E1 File Offset: 0x001235E1
	private void OnDisable()
	{
		if (!this.CanExecute)
		{
			return;
		}
		Action disabled = this.Disabled;
		if (disabled == null)
		{
			return;
		}
		disabled();
	}

	// Token: 0x0600426B RID: 17003 RVA: 0x001253FC File Offset: 0x001235FC
	private void OnTransformParentChanged()
	{
		if (!this.CanExecute)
		{
			return;
		}
		Action transformParentChanged = this.TransformParentChanged;
		if (transformParentChanged == null)
		{
			return;
		}
		transformParentChanged();
	}

	// Token: 0x040043FE RID: 17406
	public bool ExecuteInEditMode;
}
