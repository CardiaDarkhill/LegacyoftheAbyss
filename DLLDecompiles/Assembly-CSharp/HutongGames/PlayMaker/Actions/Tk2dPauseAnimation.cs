using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B65 RID: 2917
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Pause a sprite animation. Can work everyframe to pause resume animation on the fly. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W720")]
	public class Tk2dPauseAnimation : FsmStateAction
	{
		// Token: 0x06005A9C RID: 23196 RVA: 0x001CA2CC File Offset: 0x001C84CC
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005A9D RID: 23197 RVA: 0x001CA301 File Offset: 0x001C8501
		public override void Reset()
		{
			this.gameObject = null;
			this.pause = true;
			this.everyframe = false;
		}

		// Token: 0x06005A9E RID: 23198 RVA: 0x001CA31D File Offset: 0x001C851D
		public override void OnEnter()
		{
			this._getSprite();
			this.DoPauseAnimation();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A9F RID: 23199 RVA: 0x001CA339 File Offset: 0x001C8539
		public override void OnUpdate()
		{
			this.DoPauseAnimation();
		}

		// Token: 0x06005AA0 RID: 23200 RVA: 0x001CA344 File Offset: 0x001C8544
		private void DoPauseAnimation()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component: " + this._sprite.gameObject.name);
				return;
			}
			if (this._sprite.Paused != this.pause.Value)
			{
				if (this.pause.Value)
				{
					this._sprite.Pause();
					return;
				}
				this._sprite.Resume();
			}
		}

		// Token: 0x0400563C RID: 22076
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400563D RID: 22077
		[Tooltip("Pause flag")]
		public FsmBool pause;

		// Token: 0x0400563E RID: 22078
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x0400563F RID: 22079
		private tk2dSpriteAnimator _sprite;
	}
}
