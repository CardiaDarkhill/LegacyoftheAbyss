using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D95 RID: 3477
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationV2 : FsmStateAction
	{
		// Token: 0x06006517 RID: 25879 RVA: 0x001FE344 File Offset: 0x001FC544
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06006518 RID: 25880 RVA: 0x001FE379 File Offset: 0x001FC579
		public override void Reset()
		{
			this.gameObject = null;
			this.animLibName = null;
			this.clipName = null;
			this.doNotResetCurrentClip = false;
		}

		// Token: 0x06006519 RID: 25881 RVA: 0x001FE397 File Offset: 0x001FC597
		public override void OnEnter()
		{
			this._getSprite();
			this.DoPlayAnimation();
			base.Finish();
		}

		// Token: 0x0600651A RID: 25882 RVA: 0x001FE3AC File Offset: 0x001FC5AC
		private void DoPlayAnimation()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			if (this.doNotResetCurrentClip && this.clipName.Value == this._sprite.CurrentClip.name)
			{
				return;
			}
			this._sprite.Play(this.clipName.Value);
		}

		// Token: 0x04006416 RID: 25622
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006417 RID: 25623
		[Tooltip("The anim Lib name. Leave empty to use the one current selected")]
		public FsmString animLibName;

		// Token: 0x04006418 RID: 25624
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x04006419 RID: 25625
		[Tooltip("If true and requested anim clip is same as current clip, don't replay clip from the start")]
		public bool doNotResetCurrentClip;

		// Token: 0x0400641A RID: 25626
		private tk2dSpriteAnimator _sprite;
	}
}
