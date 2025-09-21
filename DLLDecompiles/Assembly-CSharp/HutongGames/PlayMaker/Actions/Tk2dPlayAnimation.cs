using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B66 RID: 2918
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimation : FsmStateAction
	{
		// Token: 0x06005AA2 RID: 23202 RVA: 0x001CA3C4 File Offset: 0x001C85C4
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
			this.heroAnim = ownerDefaultTarget.GetComponent<IHeroAnimationController>();
		}

		// Token: 0x06005AA3 RID: 23203 RVA: 0x001CA405 File Offset: 0x001C8605
		public override void Reset()
		{
			this.gameObject = null;
			this.animLibName = null;
			this.clipName = null;
		}

		// Token: 0x06005AA4 RID: 23204 RVA: 0x001CA41C File Offset: 0x001C861C
		public override void OnEnter()
		{
			this._getSprite();
			this.DoPlayAnimation();
			base.Finish();
		}

		// Token: 0x06005AA5 RID: 23205 RVA: 0x001CA430 File Offset: 0x001C8630
		private void DoPlayAnimation()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			this.animLibName.Value.Equals("");
			if (string.IsNullOrWhiteSpace(this.clipName.Value))
			{
				return;
			}
			tk2dSpriteAnimationClip clip = (this.heroAnim != null) ? this.heroAnim.GetClip(this.clipName.Value) : this._sprite.GetClipByName(this.clipName.Value);
			this._sprite.Play(clip);
		}

		// Token: 0x04005640 RID: 22080
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005641 RID: 22081
		[Tooltip("The anim Lib name. Leave empty to use the one current selected")]
		public FsmString animLibName;

		// Token: 0x04005642 RID: 22082
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x04005643 RID: 22083
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005644 RID: 22084
		private IHeroAnimationController heroAnim;
	}
}
