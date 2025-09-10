using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B77 RID: 2935
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Make a sprite pixelPerfect. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteMakePixelPerfect : FsmStateAction
	{
		// Token: 0x06005B15 RID: 23317 RVA: 0x001CBFD8 File Offset: 0x001CA1D8
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B16 RID: 23318 RVA: 0x001CC00D File Offset: 0x001CA20D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06005B17 RID: 23319 RVA: 0x001CC016 File Offset: 0x001CA216
		public override void OnEnter()
		{
			this._getSprite();
			this.MakePixelPerfect();
			base.Finish();
		}

		// Token: 0x06005B18 RID: 23320 RVA: 0x001CC02A File Offset: 0x001CA22A
		private void MakePixelPerfect()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component: ");
				return;
			}
			this._sprite.MakePixelPerfect();
		}

		// Token: 0x0400569A RID: 22170
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400569B RID: 22171
		private tk2dBaseSprite _sprite;
	}
}
