using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7B RID: 2939
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the pixel perfect flag of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetPixelPerfect : FsmStateAction
	{
		// Token: 0x06005B2B RID: 23339 RVA: 0x001CC3A8 File Offset: 0x001CA5A8
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B2C RID: 23340 RVA: 0x001CC3DD File Offset: 0x001CA5DD
		public override void Reset()
		{
			this.gameObject = null;
			this.pixelPerfect = null;
			this.everyframe = false;
		}

		// Token: 0x06005B2D RID: 23341 RVA: 0x001CC3F4 File Offset: 0x001CA5F4
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetSpritePixelPerfect();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B2E RID: 23342 RVA: 0x001CC410 File Offset: 0x001CA610
		public override void OnUpdate()
		{
			this.DoSetSpritePixelPerfect();
		}

		// Token: 0x06005B2F RID: 23343 RVA: 0x001CC418 File Offset: 0x001CA618
		private void DoSetSpritePixelPerfect()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this.pixelPerfect.Value)
			{
				this._sprite.MakePixelPerfect();
			}
		}

		// Token: 0x040056AA RID: 22186
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056AB RID: 22187
		[Tooltip("Does the sprite needs to be kept pixelPerfect? This is only necessary when using a perspective camera.")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool pixelPerfect;

		// Token: 0x040056AC RID: 22188
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056AD RID: 22189
		private tk2dBaseSprite _sprite;
	}
}
