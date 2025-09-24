using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6F RID: 2927
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Stops a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dStopAnimation : FsmStateAction
	{
		// Token: 0x06005AE3 RID: 23267 RVA: 0x001CB590 File Offset: 0x001C9790
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.LogWarning(string.Concat(new string[]
				{
					"gameObject var empty: ",
					base.Owner.name,
					"/",
					base.Fsm.Name,
					"/",
					base.State.Name
				}));
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AE4 RID: 23268 RVA: 0x001CB618 File Offset: 0x001C9818
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06005AE5 RID: 23269 RVA: 0x001CB621 File Offset: 0x001C9821
		public override void OnEnter()
		{
			this._getSprite();
			this.DoStopAnimation();
			base.Finish();
		}

		// Token: 0x06005AE6 RID: 23270 RVA: 0x001CB638 File Offset: 0x001C9838
		private void DoStopAnimation()
		{
			if (this._sprite == null)
			{
				base.LogWarning(string.Concat(new string[]
				{
					"Missing tk2dSpriteAnimator component: ",
					base.Owner.name,
					"/",
					base.Fsm.Name,
					"/",
					base.State.Name
				}));
				return;
			}
			this._sprite.Stop();
		}

		// Token: 0x04005677 RID: 22135
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005678 RID: 22136
		private tk2dSpriteAnimator _sprite;
	}
}
