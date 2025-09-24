using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C64 RID: 3172
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W721")]
	public class GetCurrentTk2dClipName : FsmStateAction
	{
		// Token: 0x06005FE3 RID: 24547 RVA: 0x001E61E8 File Offset: 0x001E43E8
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x001E621D File Offset: 0x001E441D
		public override void Reset()
		{
			this.gameObject = null;
			this.storeName = null;
		}

		// Token: 0x06005FE5 RID: 24549 RVA: 0x001E622D File Offset: 0x001E442D
		public override void OnEnter()
		{
			this._getSprite();
			this.GetClipName();
			base.Finish();
		}

		// Token: 0x06005FE6 RID: 24550 RVA: 0x001E6244 File Offset: 0x001E4444
		private void GetClipName()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			if (this._sprite.CurrentClip != null)
			{
				if (this._sprite.CurrentClip.name != null)
				{
					this.storeName.Value = this._sprite.CurrentClip.name;
					return;
				}
			}
			else if (this._sprite.DefaultClip != null && this._sprite.DefaultClip.name != null)
			{
				this.storeName.Value = this._sprite.DefaultClip.name;
			}
		}

		// Token: 0x04005D3A RID: 23866
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D3B RID: 23867
		[UIHint(UIHint.Variable)]
		public FsmString storeName;

		// Token: 0x04005D3C RID: 23868
		private tk2dSpriteAnimator _sprite;
	}
}
