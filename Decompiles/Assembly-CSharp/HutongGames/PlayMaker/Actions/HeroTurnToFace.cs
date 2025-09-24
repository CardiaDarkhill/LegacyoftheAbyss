using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001250 RID: 4688
	public class HeroTurnToFace : FsmStateAction
	{
		// Token: 0x06007BE3 RID: 31715 RVA: 0x00250C0B File Offset: 0x0024EE0B
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007BE4 RID: 31716 RVA: 0x00250C14 File Offset: 0x0024EE14
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
				base.Finish();
				return;
			}
			this.hc.StopAnimationControl();
			this.heroAnimator = this.hc.GetComponent<tk2dSpriteAnimator>();
			this.heroAnimator.Play("TurnWalk");
			this.hc.FlipSprite();
			tk2dSpriteAnimator tk2dSpriteAnimator = this.heroAnimator;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnHeroAnimationCompleted));
		}

		// Token: 0x06007BE5 RID: 31717 RVA: 0x00250D03 File Offset: 0x0024EF03
		public override void OnExit()
		{
			if (this.heroAnimator)
			{
				tk2dSpriteAnimator tk2dSpriteAnimator = this.heroAnimator;
				tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnHeroAnimationCompleted));
				this.heroAnimator = null;
			}
		}

		// Token: 0x06007BE6 RID: 31718 RVA: 0x00250D40 File Offset: 0x0024EF40
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
			this.hc.StartAnimationControl();
			if (this.hc.hero_state == ActorStates.no_input)
			{
				this.hc.AnimCtrl.PlayIdle();
			}
			base.Finish();
			tk2dSpriteAnimator tk2dSpriteAnimator = this.heroAnimator;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnHeroAnimationCompleted));
			this.heroAnimator = null;
		}

		// Token: 0x04007C0F RID: 31759
		private const string TURN_ANIM_NAME = "TurnWalk";

		// Token: 0x04007C10 RID: 31760
		public FsmOwnerDefault Target;

		// Token: 0x04007C11 RID: 31761
		private tk2dSpriteAnimator heroAnimator;

		// Token: 0x04007C12 RID: 31762
		private HeroController hc;
	}
}
