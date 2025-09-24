using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C65 RID: 3173
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	public class GetDefaultTk2dClipName : FsmStateAction
	{
		// Token: 0x06005FE8 RID: 24552 RVA: 0x001E62E8 File Offset: 0x001E44E8
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005FE9 RID: 24553 RVA: 0x001E631D File Offset: 0x001E451D
		public override void Reset()
		{
			this.gameObject = null;
			this.storeName = null;
		}

		// Token: 0x06005FEA RID: 24554 RVA: 0x001E632D File Offset: 0x001E452D
		public override void OnEnter()
		{
			this._getSprite();
			this.GetClipName();
			base.Finish();
		}

		// Token: 0x06005FEB RID: 24555 RVA: 0x001E6344 File Offset: 0x001E4544
		private void GetClipName()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			if (this._sprite.DefaultClip != null && this._sprite.DefaultClip.name != null)
			{
				this.storeName.Value = this._sprite.DefaultClip.name;
			}
		}

		// Token: 0x04005D3D RID: 23869
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D3E RID: 23870
		[UIHint(UIHint.Variable)]
		public FsmString storeName;

		// Token: 0x04005D3F RID: 23871
		private tk2dSpriteAnimator _sprite;
	}
}
