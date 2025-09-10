using System;
using HutongGames.PlayMaker;

// Token: 0x0200003C RID: 60
public class tk2dGetClipDuration : FSMUtility.GetComponentFsmStateAction<tk2dSpriteAnimator>
{
	// Token: 0x0600019D RID: 413 RVA: 0x00008CB8 File Offset: 0x00006EB8
	public override void Reset()
	{
		base.Reset();
		this.ClipName = null;
		this.StoreDuration = null;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00008CD0 File Offset: 0x00006ED0
	protected override void DoAction(tk2dSpriteAnimator component)
	{
		tk2dSpriteAnimationClip clipByName = component.GetClipByName(this.ClipName.Value);
		if (clipByName != null)
		{
			this.StoreDuration.Value = clipByName.Duration;
		}
	}

	// Token: 0x04000172 RID: 370
	[RequiredField]
	public FsmString ClipName;

	// Token: 0x04000173 RID: 371
	[UIHint(UIHint.Variable)]
	public FsmFloat StoreDuration;
}
