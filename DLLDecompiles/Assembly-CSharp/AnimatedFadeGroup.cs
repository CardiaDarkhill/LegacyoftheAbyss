using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200005E RID: 94
public sealed class AnimatedFadeGroup : MonoBehaviour
{
	// Token: 0x06000265 RID: 613 RVA: 0x0000E5EC File Offset: 0x0000C7EC
	private void OnValidate()
	{
		if (this.nestedFadeGroup == null)
		{
			this.nestedFadeGroup = base.GetComponent<NestedFadeGroupBase>();
		}
		if (!Application.isPlaying && this.nestedFadeGroup)
		{
			this.nestedFadeGroup.AlphaSelf = this.alpha;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000E638 File Offset: 0x0000C838
	private void OnEnable()
	{
		if (this.nestedFadeGroup)
		{
			this.currentAlpha = this.nestedFadeGroup.AlphaSelf;
		}
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000E658 File Offset: 0x0000C858
	private void LateUpdate()
	{
		if (!Mathf.Approximately(this.currentAlpha, this.alpha))
		{
			this.currentAlpha = Mathf.MoveTowards(this.currentAlpha, this.alpha, this.maxChangeRate * Time.deltaTime);
			this.SetAlpha(this.currentAlpha);
			return;
		}
		base.enabled = false;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000E6AF File Offset: 0x0000C8AF
	private void OnDidApplyAnimationProperties()
	{
		if (!Mathf.Approximately(this.currentAlpha, this.alpha))
		{
			base.enabled = true;
			return;
		}
		this.SetAlpha(this.alpha);
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000E6D8 File Offset: 0x0000C8D8
	public void SetTargetAlpha(float targetAlpha)
	{
		this.alpha = Mathf.Clamp01(targetAlpha);
		if (!Mathf.Approximately(this.currentAlpha, this.alpha))
		{
			base.enabled = true;
		}
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0000E700 File Offset: 0x0000C900
	private void SetAlpha(float alpha)
	{
		if (this.nestedFadeGroup)
		{
			NestedFadeGroupBase nestedFadeGroupBase = this.nestedFadeGroup;
			this.currentAlpha = alpha;
			nestedFadeGroupBase.AlphaSelf = alpha;
			return;
		}
		base.enabled = false;
	}

	// Token: 0x04000210 RID: 528
	[SerializeField]
	private NestedFadeGroupBase nestedFadeGroup;

	// Token: 0x04000211 RID: 529
	[SerializeField]
	[Range(0f, 1f)]
	private float alpha = 1f;

	// Token: 0x04000212 RID: 530
	[SerializeField]
	private float maxChangeRate = 5f;

	// Token: 0x04000213 RID: 531
	private float currentAlpha;
}
