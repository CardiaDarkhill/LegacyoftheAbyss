using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D93 RID: 3475
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationDelay : FsmStateAction
	{
		// Token: 0x0600650C RID: 25868 RVA: 0x001FE14C File Offset: 0x001FC34C
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x0600650D RID: 25869 RVA: 0x001FE181 File Offset: 0x001FC381
		public override void Reset()
		{
			this.gameObject = null;
			this.animLibName = null;
			this.clipName = null;
			this.delay = null;
		}

		// Token: 0x0600650E RID: 25870 RVA: 0x001FE19F File Offset: 0x001FC39F
		public override void OnEnter()
		{
			this.timer = 0f;
			this._getSprite();
		}

		// Token: 0x0600650F RID: 25871 RVA: 0x001FE1B2 File Offset: 0x001FC3B2
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.DoPlayAnimation();
			base.Finish();
		}

		// Token: 0x06006510 RID: 25872 RVA: 0x001FE1E8 File Offset: 0x001FC3E8
		private void DoPlayAnimation()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			this.animLibName.Value.Equals("");
			this._sprite.Play(this.clipName.Value);
		}

		// Token: 0x0400640A RID: 25610
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400640B RID: 25611
		[Tooltip("The anim Lib name. Leave empty to use the one current selected")]
		public FsmString animLibName;

		// Token: 0x0400640C RID: 25612
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x0400640D RID: 25613
		public FsmFloat delay;

		// Token: 0x0400640E RID: 25614
		private float timer;

		// Token: 0x0400640F RID: 25615
		private tk2dSpriteAnimator _sprite;
	}
}
