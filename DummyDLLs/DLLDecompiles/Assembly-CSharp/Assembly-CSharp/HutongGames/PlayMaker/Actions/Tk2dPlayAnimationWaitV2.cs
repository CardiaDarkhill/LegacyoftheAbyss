using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B68 RID: 2920
	public class Tk2dPlayAnimationWaitV2 : FsmStateAction
	{
		// Token: 0x06005AAD RID: 23213 RVA: 0x001CA637 File Offset: 0x001C8837
		public override void Reset()
		{
			this.Target = null;
			this.ClipName = null;
			this.AnimationCompleteEvent = null;
		}

		// Token: 0x06005AAE RID: 23214 RVA: 0x001CA650 File Offset: 0x001C8850
		public override void OnEnter()
		{
			this.GetSprite();
			if (this.sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				base.Finish();
				this.isValid = false;
				return;
			}
			this.isValid = true;
			IHeroAnimationController component = this.sprite.GetComponent<IHeroAnimationController>();
			if (component != null)
			{
				this.expectedClip = component.GetClip(this.ClipName.Value);
				this.sprite.Play(this.expectedClip);
			}
			else
			{
				this.expectedClip = this.sprite.GetClipByName(this.ClipName.Value);
				this.sprite.Play(this.expectedClip);
			}
			this.hasExpectedClip = (this.expectedClip != null);
			this.sprite.PlayFromFrame(0);
			tk2dSpriteAnimator tk2dSpriteAnimator = this.sprite;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate));
		}

		// Token: 0x06005AAF RID: 23215 RVA: 0x001CA739 File Offset: 0x001C8939
		public override void OnExit()
		{
			if (this.sprite == null)
			{
				return;
			}
			tk2dSpriteAnimator tk2dSpriteAnimator = this.sprite;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate));
		}

		// Token: 0x06005AB0 RID: 23216 RVA: 0x001CA771 File Offset: 0x001C8971
		public override void OnUpdate()
		{
			if (!this.hasExpectedClip)
			{
				base.Finish();
				return;
			}
			if (this.sprite.CurrentClip != this.expectedClip)
			{
				base.Fsm.Event(this.AnimationCompleteEvent);
				base.Finish();
			}
		}

		// Token: 0x06005AB1 RID: 23217 RVA: 0x001CA7AC File Offset: 0x001C89AC
		private void GetSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x001CA7E1 File Offset: 0x001C89E1
		private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
		{
			if (clip.name != this.ClipName.Value)
			{
				return;
			}
			base.Fsm.Event(this.AnimationCompleteEvent);
			base.Finish();
		}

		// Token: 0x04005649 RID: 22089
		[RequiredField]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault Target;

		// Token: 0x0400564A RID: 22090
		[RequiredField]
		public FsmString ClipName;

		// Token: 0x0400564B RID: 22091
		public FsmEvent AnimationCompleteEvent;

		// Token: 0x0400564C RID: 22092
		private tk2dSpriteAnimator sprite;

		// Token: 0x0400564D RID: 22093
		private bool isValid;

		// Token: 0x0400564E RID: 22094
		private bool hasExpectedClip;

		// Token: 0x0400564F RID: 22095
		private tk2dSpriteAnimationClip expectedClip;
	}
}
