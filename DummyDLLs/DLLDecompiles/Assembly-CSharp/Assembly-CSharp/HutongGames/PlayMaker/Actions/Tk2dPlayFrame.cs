using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD4 RID: 3284
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Goto a specific frame for current animation.")]
	public class Tk2dPlayFrame : FsmStateAction
	{
		// Token: 0x060061DF RID: 25055 RVA: 0x001EF590 File Offset: 0x001ED790
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x060061E0 RID: 25056 RVA: 0x001EF5C5 File Offset: 0x001ED7C5
		public override void Reset()
		{
			this.gameObject = null;
			this.frame = 0;
		}

		// Token: 0x060061E1 RID: 25057 RVA: 0x001EF5DA File Offset: 0x001ED7DA
		public override void OnEnter()
		{
			this._getSprite();
			if (this._sprite)
			{
				this._sprite.PlayFromFrame(this.frame.Value);
			}
			base.Finish();
		}

		// Token: 0x04005FFD RID: 24573
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005FFE RID: 24574
		[RequiredField]
		public FsmInt frame;

		// Token: 0x04005FFF RID: 24575
		private tk2dSpriteAnimator _sprite;
	}
}
