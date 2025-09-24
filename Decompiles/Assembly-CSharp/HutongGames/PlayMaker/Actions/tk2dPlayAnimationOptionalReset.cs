using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D94 RID: 3476
	public class tk2dPlayAnimationOptionalReset : FsmStateAction
	{
		// Token: 0x06006512 RID: 25874 RVA: 0x001FE243 File Offset: 0x001FC443
		public override void Reset()
		{
			this.Target = null;
			this.AnimName = null;
			this.ResetFrame = null;
			this.EveryFrame = true;
		}

		// Token: 0x06006513 RID: 25875 RVA: 0x001FE264 File Offset: 0x001FC464
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.animator = safe.GetComponent<tk2dSpriteAnimator>();
				this.heroAnim = safe.GetComponent<IHeroAnimationController>();
			}
			if (!this.animator)
			{
				base.Finish();
				return;
			}
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006514 RID: 25876 RVA: 0x001FE2C6 File Offset: 0x001FC4C6
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06006515 RID: 25877 RVA: 0x001FE2D0 File Offset: 0x001FC4D0
		private void DoAction()
		{
			tk2dSpriteAnimationClip clip = (this.heroAnim != null) ? this.heroAnim.GetClip(this.AnimName.Value) : this.animator.GetClipByName(this.AnimName.Value);
			if (this.ResetFrame.Value)
			{
				this.animator.PlayFromFrame(clip, 0);
				return;
			}
			this.animator.Play(clip);
		}

		// Token: 0x04006410 RID: 25616
		public FsmOwnerDefault Target;

		// Token: 0x04006411 RID: 25617
		public FsmString AnimName;

		// Token: 0x04006412 RID: 25618
		public FsmBool ResetFrame;

		// Token: 0x04006413 RID: 25619
		public bool EveryFrame;

		// Token: 0x04006414 RID: 25620
		private tk2dSpriteAnimator animator;

		// Token: 0x04006415 RID: 25621
		private IHeroAnimationController heroAnim;
	}
}
