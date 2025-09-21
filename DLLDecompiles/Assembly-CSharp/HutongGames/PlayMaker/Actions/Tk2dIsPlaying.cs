using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B64 RID: 2916
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Check if a sprite animation is playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W720")]
	public class Tk2dIsPlaying : FsmStateAction
	{
		// Token: 0x06005A96 RID: 23190 RVA: 0x001CA1BC File Offset: 0x001C83BC
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005A97 RID: 23191 RVA: 0x001CA1F1 File Offset: 0x001C83F1
		public override void Reset()
		{
			this.gameObject = null;
			this.clipName = null;
			this.everyframe = false;
			this.isPlayingEvent = null;
			this.isNotPlayingEvent = null;
		}

		// Token: 0x06005A98 RID: 23192 RVA: 0x001CA216 File Offset: 0x001C8416
		public override void OnEnter()
		{
			this._getSprite();
			this.DoIsPlaying();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A99 RID: 23193 RVA: 0x001CA232 File Offset: 0x001C8432
		public override void OnUpdate()
		{
			this.DoIsPlaying();
		}

		// Token: 0x06005A9A RID: 23194 RVA: 0x001CA23C File Offset: 0x001C843C
		private void DoIsPlaying()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component: " + this._sprite.gameObject.name);
				return;
			}
			bool flag = this._sprite.IsPlaying(this.clipName.Value);
			this.isPlaying.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isPlayingEvent);
				return;
			}
			base.Fsm.Event(this.isNotPlayingEvent);
		}

		// Token: 0x04005635 RID: 22069
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005636 RID: 22070
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x04005637 RID: 22071
		[Tooltip("is the clip playing?")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlaying;

		// Token: 0x04005638 RID: 22072
		[Tooltip("EVvnt sent if clip is playing")]
		public FsmEvent isPlayingEvent;

		// Token: 0x04005639 RID: 22073
		[Tooltip("Event sent if clip is not playing")]
		public FsmEvent isNotPlayingEvent;

		// Token: 0x0400563A RID: 22074
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x0400563B RID: 22075
		private tk2dSpriteAnimator _sprite;
	}
}
