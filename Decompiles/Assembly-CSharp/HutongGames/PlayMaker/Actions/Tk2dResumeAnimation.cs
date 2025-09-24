using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6D RID: 2925
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Resume a sprite animation. Use Tk2dPauseAnimation for dynamic control. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W721")]
	public class Tk2dResumeAnimation : FsmStateAction
	{
		// Token: 0x06005AD8 RID: 23256 RVA: 0x001CB448 File Offset: 0x001C9648
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AD9 RID: 23257 RVA: 0x001CB47D File Offset: 0x001C967D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06005ADA RID: 23258 RVA: 0x001CB486 File Offset: 0x001C9686
		public override void OnEnter()
		{
			this._getSprite();
			this.DoResumeAnimation();
			base.Finish();
		}

		// Token: 0x06005ADB RID: 23259 RVA: 0x001CB49A File Offset: 0x001C969A
		private void DoResumeAnimation()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			if (this._sprite.Paused)
			{
				this._sprite.Resume();
			}
		}

		// Token: 0x04005671 RID: 22129
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005672 RID: 22130
		private tk2dSpriteAnimator _sprite;
	}
}
