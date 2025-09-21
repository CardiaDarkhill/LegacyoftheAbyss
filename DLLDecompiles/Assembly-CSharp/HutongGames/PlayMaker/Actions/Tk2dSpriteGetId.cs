using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B74 RID: 2932
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the sprite id of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
	public class Tk2dSpriteGetId : FsmStateAction
	{
		// Token: 0x06005B06 RID: 23302 RVA: 0x001CBE14 File Offset: 0x001CA014
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x001CBE49 File Offset: 0x001CA049
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteID = null;
			this.everyframe = false;
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x001CBE60 File Offset: 0x001CA060
		public override void OnEnter()
		{
			this._getSprite();
			this.DoGetSpriteID();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B09 RID: 23305 RVA: 0x001CBE7C File Offset: 0x001CA07C
		public override void OnUpdate()
		{
			this.DoGetSpriteID();
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x001CBE84 File Offset: 0x001CA084
		private void DoGetSpriteID()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this.spriteID.Value != this._sprite.spriteId)
			{
				this.spriteID.Value = this._sprite.spriteId;
			}
		}

		// Token: 0x04005690 RID: 22160
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005691 RID: 22161
		[Tooltip("The sprite Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt spriteID;

		// Token: 0x04005692 RID: 22162
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x04005693 RID: 22163
		private tk2dBaseSprite _sprite;
	}
}
