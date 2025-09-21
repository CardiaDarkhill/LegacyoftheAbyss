using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B73 RID: 2931
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the color of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteGetColor : FsmStateAction
	{
		// Token: 0x06005B00 RID: 23296 RVA: 0x001CBD40 File Offset: 0x001C9F40
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B01 RID: 23297 RVA: 0x001CBD75 File Offset: 0x001C9F75
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
			this.everyframe = false;
		}

		// Token: 0x06005B02 RID: 23298 RVA: 0x001CBD8C File Offset: 0x001C9F8C
		public override void OnEnter()
		{
			this._getSprite();
			this.DoGetSpriteColor();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B03 RID: 23299 RVA: 0x001CBDA8 File Offset: 0x001C9FA8
		public override void OnUpdate()
		{
			this.DoGetSpriteColor();
		}

		// Token: 0x06005B04 RID: 23300 RVA: 0x001CBDB0 File Offset: 0x001C9FB0
		private void DoGetSpriteColor()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this._sprite.color != this.color.Value)
			{
				this.color.Value = this._sprite.color;
			}
		}

		// Token: 0x0400568C RID: 22156
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400568D RID: 22157
		[Tooltip("The color")]
		[UIHint(UIHint.Variable)]
		public FsmColor color;

		// Token: 0x0400568E RID: 22158
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x0400568F RID: 22159
		private tk2dBaseSprite _sprite;
	}
}
