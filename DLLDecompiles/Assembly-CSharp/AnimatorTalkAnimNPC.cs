using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200062A RID: 1578
public class AnimatorTalkAnimNPC : TalkAnimNPC
{
	// Token: 0x06003831 RID: 14385 RVA: 0x000F8451 File Offset: 0x000F6651
	protected override void Awake()
	{
		base.Awake();
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06003832 RID: 14386 RVA: 0x000F8465 File Offset: 0x000F6665
	protected override void PlayAnim(string animName)
	{
		this.animator.Play(animName);
	}

	// Token: 0x06003833 RID: 14387 RVA: 0x000F8473 File Offset: 0x000F6673
	protected override IEnumerator PlayAnimWait(string animName)
	{
		this.animator.Play(animName);
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield break;
	}

	// Token: 0x04003B29 RID: 15145
	private Animator animator;
}
