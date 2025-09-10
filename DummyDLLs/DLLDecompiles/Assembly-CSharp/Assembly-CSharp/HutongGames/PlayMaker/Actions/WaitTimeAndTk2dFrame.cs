using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA7 RID: 3495
	public class WaitTimeAndTk2dFrame : FsmStateAction
	{
		// Token: 0x06006578 RID: 25976 RVA: 0x00200410 File Offset: 0x001FE610
		public override void Reset()
		{
			this.Tk2dAnimator = null;
			this.Time = 1f;
			this.FinishEvent = null;
		}

		// Token: 0x06006579 RID: 25977 RVA: 0x00200430 File Offset: 0x001FE630
		public override void OnEnter()
		{
			if (this.Time.Value <= 0f)
			{
				base.Fsm.Event(this.FinishEvent);
				base.Finish();
				return;
			}
			this.timer = 0f;
			this.queuedEnd = false;
			GameObject safe = this.Tk2dAnimator.GetSafe(this);
			this.animator = (safe ? safe.GetComponent<tk2dSpriteAnimator>() : null);
			if (this.animator == null)
			{
				Debug.LogError("Tk2d animator was null", base.Owner);
				base.Finish();
			}
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.FrameChanged = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>)Delegate.Combine(tk2dSpriteAnimator.FrameChanged, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>(this.OnFrameChanged));
			tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
			tk2dSpriteAnimator2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		}

		// Token: 0x0600657A RID: 25978 RVA: 0x00200510 File Offset: 0x001FE710
		public override void OnUpdate()
		{
			if (this.queuedEnd)
			{
				return;
			}
			this.timer += UnityEngine.Time.deltaTime;
			if (this.timer >= this.Time.Value)
			{
				if (this.animator.Playing)
				{
					this.queuedEnd = true;
					return;
				}
				this.End();
			}
		}

		// Token: 0x0600657B RID: 25979 RVA: 0x00200568 File Offset: 0x001FE768
		public override void OnExit()
		{
			if (this.animator)
			{
				tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
				tk2dSpriteAnimator.FrameChanged = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>)Delegate.Remove(tk2dSpriteAnimator.FrameChanged, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>(this.OnFrameChanged));
				tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
				tk2dSpriteAnimator2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
			}
		}

		// Token: 0x0600657C RID: 25980 RVA: 0x002005D0 File Offset: 0x001FE7D0
		private void End()
		{
			base.Finish();
			if (this.FinishEvent != null)
			{
				base.Fsm.Event(this.FinishEvent);
			}
		}

		// Token: 0x0600657D RID: 25981 RVA: 0x002005F1 File Offset: 0x001FE7F1
		private void OnFrameChanged(tk2dSpriteAnimator eventAnimator, tk2dSpriteAnimationClip clip, int previousFrame, int currentFrame)
		{
			if (!this.queuedEnd)
			{
				return;
			}
			this.End();
		}

		// Token: 0x0600657E RID: 25982 RVA: 0x00200602 File Offset: 0x001FE802
		private void OnAnimationCompleted(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
		{
			if (!this.queuedEnd)
			{
				return;
			}
			this.End();
		}

		// Token: 0x0400647D RID: 25725
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault Tk2dAnimator;

		// Token: 0x0400647E RID: 25726
		[RequiredField]
		public FsmFloat Time;

		// Token: 0x0400647F RID: 25727
		public FsmEvent FinishEvent;

		// Token: 0x04006480 RID: 25728
		private float timer;

		// Token: 0x04006481 RID: 25729
		private bool queuedEnd;

		// Token: 0x04006482 RID: 25730
		private tk2dSpriteAnimator animator;
	}
}
