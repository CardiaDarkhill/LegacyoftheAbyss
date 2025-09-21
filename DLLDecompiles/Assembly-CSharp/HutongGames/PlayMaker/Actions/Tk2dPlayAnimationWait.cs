using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B67 RID: 2919
	public class Tk2dPlayAnimationWait : FsmStateAction
	{
		// Token: 0x06005AA7 RID: 23207 RVA: 0x001CA4CB File Offset: 0x001C86CB
		public override void Reset()
		{
			this.Target = null;
			this.ClipName = null;
			this.AnimationCompleteEvent = null;
		}

		// Token: 0x06005AA8 RID: 23208 RVA: 0x001CA4E4 File Offset: 0x001C86E4
		public override void OnEnter()
		{
			this.GetSprite();
			if (this.sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				base.Finish();
				return;
			}
			IHeroAnimationController component = this.sprite.GetComponent<IHeroAnimationController>();
			if (component != null)
			{
				this.sprite.Play(component.GetClip(this.ClipName.Value));
			}
			else
			{
				this.sprite.Play(this.ClipName.Value);
			}
			this.sprite.PlayFromFrame(0);
			tk2dSpriteAnimator tk2dSpriteAnimator = this.sprite;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate));
		}

		// Token: 0x06005AA9 RID: 23209 RVA: 0x001CA58D File Offset: 0x001C878D
		public override void OnExit()
		{
			if (this.sprite == null)
			{
				return;
			}
			tk2dSpriteAnimator tk2dSpriteAnimator = this.sprite;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate));
		}

		// Token: 0x06005AAA RID: 23210 RVA: 0x001CA5C8 File Offset: 0x001C87C8
		private void GetSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AAB RID: 23211 RVA: 0x001CA5FD File Offset: 0x001C87FD
		private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
		{
			if (clip.name != this.ClipName.Value)
			{
				return;
			}
			base.Fsm.Event(this.AnimationCompleteEvent);
			base.Finish();
		}

		// Token: 0x04005645 RID: 22085
		[RequiredField]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault Target;

		// Token: 0x04005646 RID: 22086
		[RequiredField]
		public FsmString ClipName;

		// Token: 0x04005647 RID: 22087
		public FsmEvent AnimationCompleteEvent;

		// Token: 0x04005648 RID: 22088
		private tk2dSpriteAnimator sprite;
	}
}
