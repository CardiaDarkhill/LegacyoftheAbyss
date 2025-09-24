using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class AppearAnimator : MonoBehaviour
{
	// Token: 0x0600029E RID: 670 RVA: 0x0000EFD9 File Offset: 0x0000D1D9
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000EFF3 File Offset: 0x0000D1F3
	public void Disappear()
	{
		base.StopAllCoroutines();
		if (this.animator)
		{
			this.animator.enabled = false;
		}
		if (this.spriteRenderer)
		{
			this.spriteRenderer.enabled = false;
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0000F02D File Offset: 0x0000D22D
	public void Appear()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.AppearRoutine());
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0000F042 File Offset: 0x0000D242
	private IEnumerator AppearRoutine()
	{
		if (this.spriteRenderer)
		{
			this.spriteRenderer.enabled = true;
			this.spriteRenderer.sprite = null;
		}
		if (this.animator)
		{
			AnimatorCullingMode culling = this.animator.cullingMode;
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.enabled = true;
			this.animator.Play(AppearAnimator._appearAnimId);
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
			this.animator.cullingMode = culling;
		}
		yield break;
	}

	// Token: 0x0400023E RID: 574
	[SerializeField]
	private Animator animator;

	// Token: 0x0400023F RID: 575
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04000240 RID: 576
	private static readonly int _appearAnimId = Animator.StringToHash("Appear");
}
