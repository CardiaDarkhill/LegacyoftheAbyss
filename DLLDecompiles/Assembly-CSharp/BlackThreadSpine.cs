using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200056E RID: 1390
public class BlackThreadSpine : MonoBehaviour
{
	// Token: 0x060031AF RID: 12719 RVA: 0x000DC7DA File Offset: 0x000DA9DA
	private void Awake()
	{
		this.UpdateInitialScale();
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x000DC7E2 File Offset: 0x000DA9E2
	private void OnEnable()
	{
		this.waitFrames = 0;
	}

	// Token: 0x060031B1 RID: 12721 RVA: 0x000DC7EC File Offset: 0x000DA9EC
	private void Update()
	{
		if (this.waitFrames > 0)
		{
			this.waitFrames--;
			if (this.waitFrames > 0)
			{
				return;
			}
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - Mathf.Epsilon)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x060031B2 RID: 12722 RVA: 0x000DC846 File Offset: 0x000DAA46
	public void UpdateInitialScale()
	{
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x060031B3 RID: 12723 RVA: 0x000DC85C File Offset: 0x000DAA5C
	public void Spawned(bool isNeedolinPlaying)
	{
		this.animator.Play(BlackThreadSpine._appearAnim);
		this.waitFrames = 1;
		base.transform.FlipLocalScale(this.doRandomFlipX && Random.Range(0, 2) == 0, this.doRandomFlipY && Random.Range(0, 2) == 0, false);
		if (this.needolinSpeed.IsEnabled)
		{
			this.animator.SetFloat(BlackThreadSpine._speedProp, isNeedolinPlaying ? this.needolinSpeed.Value : 1f);
		}
		float num = this.initialScale.y * this.scaleRangeY.GetRandomValue();
		Vector3 localScale = base.transform.localScale;
		if (Math.Abs(localScale.y - num) > Mathf.Epsilon)
		{
			localScale.y = num;
			base.transform.localScale = localScale;
		}
	}

	// Token: 0x04003515 RID: 13589
	private static readonly int _appearAnim = Animator.StringToHash("Appear");

	// Token: 0x04003516 RID: 13590
	private static readonly int _speedProp = Animator.StringToHash("Speed");

	// Token: 0x04003517 RID: 13591
	[SerializeField]
	private Animator animator;

	// Token: 0x04003518 RID: 13592
	[SerializeField]
	private bool doRandomFlipX = true;

	// Token: 0x04003519 RID: 13593
	[SerializeField]
	private bool doRandomFlipY = true;

	// Token: 0x0400351A RID: 13594
	[SerializeField]
	private OverrideFloat needolinSpeed;

	// Token: 0x0400351B RID: 13595
	[SerializeField]
	private MinMaxFloat scaleRangeY = new MinMaxFloat(1f, 1f);

	// Token: 0x0400351C RID: 13596
	private int waitFrames;

	// Token: 0x0400351D RID: 13597
	private Vector3 initialScale;
}
