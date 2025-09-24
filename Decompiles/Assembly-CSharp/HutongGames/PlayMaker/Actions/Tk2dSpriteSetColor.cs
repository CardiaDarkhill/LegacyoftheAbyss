using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B78 RID: 2936
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the color of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetColor : FsmStateAction
	{
		// Token: 0x06005B1A RID: 23322 RVA: 0x001CC05C File Offset: 0x001CA25C
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B1B RID: 23323 RVA: 0x001CC091 File Offset: 0x001CA291
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
			this.everyframe = false;
		}

		// Token: 0x06005B1C RID: 23324 RVA: 0x001CC0A8 File Offset: 0x001CA2A8
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetSpriteColor();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B1D RID: 23325 RVA: 0x001CC0C4 File Offset: 0x001CA2C4
		public override void OnUpdate()
		{
			this.DoSetSpriteColor();
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x001CC0CC File Offset: 0x001CA2CC
		private void DoSetSpriteColor()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this._sprite.color != this.color.Value)
			{
				this._sprite.color = this.color.Value;
			}
		}

		// Token: 0x0400569C RID: 22172
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400569D RID: 22173
		[Tooltip("The color")]
		[UIHint(UIHint.FsmColor)]
		public FsmColor color;

		// Token: 0x0400569E RID: 22174
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x0400569F RID: 22175
		private tk2dBaseSprite _sprite;
	}
}
