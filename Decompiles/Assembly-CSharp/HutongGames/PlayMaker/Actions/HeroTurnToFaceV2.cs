using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001251 RID: 4689
	public class HeroTurnToFaceV2 : FsmStateAction
	{
		// Token: 0x06007BE8 RID: 31720 RVA: 0x00250DEC File Offset: 0x0024EFEC
		public override void Reset()
		{
			this.Target = null;
			this.didNotTurnEvent = null;
			this.didTurnEvent = null;
			this.turnFinishedEvent = null;
		}

		// Token: 0x06007BE9 RID: 31721 RVA: 0x00250E0C File Offset: 0x0024F00C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.hc = HeroController.instance;
			Transform transform = this.hc.transform;
			float num = safe.transform.position.x - transform.position.x;
			float x = transform.localScale.x;
			bool flag = false;
			if (x > 0f)
			{
				if (num > 0f)
				{
					flag = true;
				}
			}
			else if (num < 0f)
			{
				flag = true;
			}
			if (!flag)
			{
				base.Fsm.Event(this.didNotTurnEvent);
				base.Finish();
				return;
			}
			this.hc.StopAnimationControl();
			this.heroAnimator = this.hc.GetComponent<tk2dSpriteAnimator>();
			this.heroAnimator.Play("TurnWalk");
			this.hc.FlipSprite();
			this.heroAnimator.AnimationCompletedEvent += this.OnHeroAnimationCompleted;
			this.heroAnimator.AnimationChanged += this.OnAnimationChanged;
			base.Fsm.Event(this.didTurnEvent);
		}

		// Token: 0x06007BEA RID: 31722 RVA: 0x00250F24 File Offset: 0x0024F124
		private void OnAnimationChanged(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip previousclip, tk2dSpriteAnimationClip newclip)
		{
			if (newclip.name != "TurnWalk")
			{
				base.Fsm.Event(this.turnFinishedEvent);
				this.hc.StartAnimationControl();
				base.Finish();
				this.heroAnimator.AnimationCompletedEvent -= this.OnHeroAnimationCompleted;
				this.heroAnimator.AnimationChanged -= this.OnAnimationChanged;
				this.heroAnimator = null;
			}
		}

		// Token: 0x06007BEB RID: 31723 RVA: 0x00250F9C File Offset: 0x0024F19C
		public override void OnExit()
		{
			if (this.heroAnimator)
			{
				this.heroAnimator.AnimationCompletedEvent -= this.OnHeroAnimationCompleted;
				this.heroAnimator.AnimationChanged -= this.OnAnimationChanged;
				this.heroAnimator = null;
			}
		}

		// Token: 0x06007BEC RID: 31724 RVA: 0x00250FEC File Offset: 0x0024F1EC
		private void OnHeroAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
		{
			if (clip.name != "TurnWalk")
			{
				Debug.LogErrorFormat(base.Owner, "Wrong animation finished! Expected: {0}, Was: {1}", new object[]
				{
					"TurnWalk",
					clip.name
				});
				return;
			}
			base.Fsm.Event(this.turnFinishedEvent);
			this.hc.StartAnimationControl();
			if (this.hc.hero_state == ActorStates.no_input)
			{
				this.hc.AnimCtrl.PlayIdle();
			}
			base.Finish();
			this.heroAnimator.AnimationCompletedEvent -= this.OnHeroAnimationCompleted;
			this.heroAnimator.AnimationChanged -= this.OnAnimationChanged;
			this.heroAnimator = null;
		}

		// Token: 0x04007C13 RID: 31763
		private const string TURN_ANIM_NAME = "TurnWalk";

		// Token: 0x04007C14 RID: 31764
		public FsmOwnerDefault Target;

		// Token: 0x04007C15 RID: 31765
		public FsmEvent didNotTurnEvent;

		// Token: 0x04007C16 RID: 31766
		public FsmEvent didTurnEvent;

		// Token: 0x04007C17 RID: 31767
		public FsmEvent turnFinishedEvent;

		// Token: 0x04007C18 RID: 31768
		private tk2dSpriteAnimator heroAnimator;

		// Token: 0x04007C19 RID: 31769
		private HeroController hc;
	}
}
