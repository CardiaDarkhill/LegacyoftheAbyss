using System;
using UnityEngine;

// Token: 0x0200074A RID: 1866
public class VisibilityControl : MonoBehaviour
{
	// Token: 0x06004272 RID: 17010 RVA: 0x00125517 File Offset: 0x00123717
	private void Awake()
	{
		this.myAnimator = base.GetComponent<Animator>();
	}

	// Token: 0x06004273 RID: 17011 RVA: 0x00125525 File Offset: 0x00123725
	public void Reveal()
	{
		if (this.controlType == VisibilityControl.ControlType.SHOW_AND_HIDE)
		{
			this.myAnimator.ResetTrigger("hide");
			this.myAnimator.SetTrigger("show");
		}
	}

	// Token: 0x06004274 RID: 17012 RVA: 0x0012554F File Offset: 0x0012374F
	public void Hide()
	{
		this.myAnimator.ResetTrigger("show");
		this.myAnimator.SetTrigger("hide");
	}

	// Token: 0x04004408 RID: 17416
	private Animator myAnimator;

	// Token: 0x04004409 RID: 17417
	public VisibilityControl.ControlType controlType;

	// Token: 0x02001A28 RID: 6696
	public enum ControlType
	{
		// Token: 0x040098D0 RID: 39120
		SHOW_AND_HIDE,
		// Token: 0x040098D1 RID: 39121
		HIDE_ONLY
	}
}
