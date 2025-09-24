using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B76 RID: 2934
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the scale of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteGetScale : FsmStateAction
	{
		// Token: 0x06005B0F RID: 23311 RVA: 0x001CBF04 File Offset: 0x001CA104
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B10 RID: 23312 RVA: 0x001CBF39 File Offset: 0x001CA139
		public override void Reset()
		{
			this.gameObject = null;
			this.scale = null;
			this.everyframe = false;
		}

		// Token: 0x06005B11 RID: 23313 RVA: 0x001CBF50 File Offset: 0x001CA150
		public override void OnEnter()
		{
			this._getSprite();
			this.DoGetSpriteScale();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B12 RID: 23314 RVA: 0x001CBF6C File Offset: 0x001CA16C
		public override void OnUpdate()
		{
			this.DoGetSpriteScale();
		}

		// Token: 0x06005B13 RID: 23315 RVA: 0x001CBF74 File Offset: 0x001CA174
		private void DoGetSpriteScale()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this._sprite.scale != this.scale.Value)
			{
				this.scale.Value = this._sprite.scale;
			}
		}

		// Token: 0x04005696 RID: 22166
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005697 RID: 22167
		[Tooltip("The scale Id")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;

		// Token: 0x04005698 RID: 22168
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x04005699 RID: 22169
		private tk2dBaseSprite _sprite;
	}
}
