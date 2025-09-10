using System;
using UnityEngine;

// Token: 0x02000703 RID: 1795
public sealed class AnimationTriggerIndicator : ScrollIndicator
{
	// Token: 0x0600401B RID: 16411 RVA: 0x0011A732 File Offset: 0x00118932
	private void OnEnable()
	{
		if (this.isShowing)
		{
			this.Show();
			return;
		}
		this.Hide();
	}

	// Token: 0x0600401C RID: 16412 RVA: 0x0011A749 File Offset: 0x00118949
	private void OnValidate()
	{
		if (!this.animator)
		{
			this.animator = base.GetComponent<Animator>();
		}
	}

	// Token: 0x0600401D RID: 16413 RVA: 0x0011A764 File Offset: 0x00118964
	public override void Show()
	{
		this.isShowing = true;
		if (this.resetTrigger)
		{
			this.animator.ResetTrigger(this.hideTrigger);
		}
		this.animator.SetTrigger(this.showTrigger);
	}

	// Token: 0x0600401E RID: 16414 RVA: 0x0011A797 File Offset: 0x00118997
	public override void Hide()
	{
		this.isShowing = false;
		if (this.resetTrigger)
		{
			this.animator.ResetTrigger(this.showTrigger);
		}
		this.animator.SetTrigger(this.hideTrigger);
	}

	// Token: 0x040041C5 RID: 16837
	[SerializeField]
	private Animator animator;

	// Token: 0x040041C6 RID: 16838
	[SerializeField]
	private string showTrigger = "show";

	// Token: 0x040041C7 RID: 16839
	[SerializeField]
	private string hideTrigger = "hide";

	// Token: 0x040041C8 RID: 16840
	[SerializeField]
	private bool resetTrigger = true;

	// Token: 0x040041C9 RID: 16841
	private bool isShowing;
}
