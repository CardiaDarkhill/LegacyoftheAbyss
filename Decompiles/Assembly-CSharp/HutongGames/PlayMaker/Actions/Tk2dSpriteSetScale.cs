using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7C RID: 2940
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the scale of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetScale : FsmStateAction
	{
		// Token: 0x06005B31 RID: 23345 RVA: 0x001CC454 File Offset: 0x001CA654
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B32 RID: 23346 RVA: 0x001CC489 File Offset: 0x001CA689
		public override void Reset()
		{
			this.gameObject = null;
			this.scale = new Vector3(1f, 1f, 1f);
			this.everyframe = false;
		}

		// Token: 0x06005B33 RID: 23347 RVA: 0x001CC4B8 File Offset: 0x001CA6B8
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetSpriteScale();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B34 RID: 23348 RVA: 0x001CC4D4 File Offset: 0x001CA6D4
		public override void OnUpdate()
		{
			this.DoSetSpriteScale();
		}

		// Token: 0x06005B35 RID: 23349 RVA: 0x001CC4DC File Offset: 0x001CA6DC
		private void DoSetSpriteScale()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (this._sprite.scale != this.scale.Value)
			{
				this._sprite.scale = this.scale.Value;
			}
		}

		// Token: 0x040056AE RID: 22190
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056AF RID: 22191
		[Tooltip("The scale Id")]
		[UIHint(UIHint.FsmVector3)]
		public FsmVector3 scale;

		// Token: 0x040056B0 RID: 22192
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056B1 RID: 22193
		private tk2dBaseSprite _sprite;
	}
}
