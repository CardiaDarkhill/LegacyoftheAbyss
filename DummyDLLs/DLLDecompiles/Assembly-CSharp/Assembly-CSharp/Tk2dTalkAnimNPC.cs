using System;
using System.Collections;

// Token: 0x02000640 RID: 1600
public class Tk2dTalkAnimNPC : TalkAnimNPC
{
	// Token: 0x06003970 RID: 14704 RVA: 0x000FC942 File Offset: 0x000FAB42
	protected override void Awake()
	{
		base.Awake();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06003971 RID: 14705 RVA: 0x000FC956 File Offset: 0x000FAB56
	protected override void PlayAnim(string animName)
	{
		this.animator.Play(animName);
	}

	// Token: 0x06003972 RID: 14706 RVA: 0x000FC964 File Offset: 0x000FAB64
	protected override IEnumerator PlayAnimWait(string animName)
	{
		return this.animator.PlayAnimWait(animName, null);
	}

	// Token: 0x04003C35 RID: 15413
	private tk2dSpriteAnimator animator;
}
