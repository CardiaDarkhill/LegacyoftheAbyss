using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6E RID: 2926
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Set the current clip frames per seconds on a animated sprite. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dSetAnimationFrameRate : FsmStateAction
	{
		// Token: 0x06005ADD RID: 23261 RVA: 0x001CB4D8 File Offset: 0x001C96D8
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005ADE RID: 23262 RVA: 0x001CB50D File Offset: 0x001C970D
		public override void Reset()
		{
			this.gameObject = null;
			this.framePerSeconds = 30f;
			this.everyFrame = false;
		}

		// Token: 0x06005ADF RID: 23263 RVA: 0x001CB52D File Offset: 0x001C972D
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetAnimationFPS();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005AE0 RID: 23264 RVA: 0x001CB549 File Offset: 0x001C9749
		public override void OnUpdate()
		{
			this.DoSetAnimationFPS();
		}

		// Token: 0x06005AE1 RID: 23265 RVA: 0x001CB551 File Offset: 0x001C9751
		private void DoSetAnimationFPS()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			this._sprite.CurrentClip.fps = this.framePerSeconds.Value;
		}

		// Token: 0x04005673 RID: 22131
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005674 RID: 22132
		[RequiredField]
		[Tooltip("The frame per seconds of the current clip")]
		public FsmFloat framePerSeconds;

		// Token: 0x04005675 RID: 22133
		[Tooltip("Repeat every Frame")]
		public bool everyFrame;

		// Token: 0x04005676 RID: 22134
		private tk2dSpriteAnimator _sprite;
	}
}
