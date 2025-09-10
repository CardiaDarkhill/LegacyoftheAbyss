using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B79 RID: 2937
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the color of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetColorRecolour : FsmStateAction
	{
		// Token: 0x06005B20 RID: 23328 RVA: 0x001CC130 File Offset: 0x001CA330
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B21 RID: 23329 RVA: 0x001CC165 File Offset: 0x001CA365
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
			this.everyframe = false;
		}

		// Token: 0x06005B22 RID: 23330 RVA: 0x001CC17C File Offset: 0x001CA37C
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetSpriteColor();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B23 RID: 23331 RVA: 0x001CC198 File Offset: 0x001CA398
		public override void OnUpdate()
		{
			this.DoSetSpriteColor();
		}

		// Token: 0x06005B24 RID: 23332 RVA: 0x001CC1A0 File Offset: 0x001CA3A0
		private void DoSetSpriteColor()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this.enableRecolour.Value)
			{
				base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<tk2dSprite>().EnableKeyword("RECOLOUR");
			}
			else
			{
				base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<tk2dSprite>().DisableKeyword("RECOLOUR");
			}
			if (this._sprite.color != this.color.Value)
			{
				this._sprite.color = this.color.Value;
			}
		}

		// Token: 0x040056A0 RID: 22176
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056A1 RID: 22177
		[Tooltip("The color")]
		[UIHint(UIHint.FsmColor)]
		public FsmColor color;

		// Token: 0x040056A2 RID: 22178
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056A3 RID: 22179
		public FsmBool enableRecolour;

		// Token: 0x040056A4 RID: 22180
		private tk2dBaseSprite _sprite;
	}
}
