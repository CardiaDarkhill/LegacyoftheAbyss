using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7A RID: 2938
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the sprite id of a sprite. Can use id or name. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetId : FsmStateAction
	{
		// Token: 0x06005B26 RID: 23334 RVA: 0x001CC254 File Offset: 0x001CA454
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x06005B27 RID: 23335 RVA: 0x001CC289 File Offset: 0x001CA489
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteID = null;
			this.ORSpriteName = null;
			this.spriteCollection = new FsmGameObject
			{
				UseVariable = true
			};
		}

		// Token: 0x06005B28 RID: 23336 RVA: 0x001CC2B2 File Offset: 0x001CA4B2
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetSpriteID();
			base.Finish();
		}

		// Token: 0x06005B29 RID: 23337 RVA: 0x001CC2C8 File Offset: 0x001CA4C8
		private void DoSetSpriteID()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dBaseSprite component: " + this._sprite.gameObject.name);
				return;
			}
			tk2dSpriteCollectionData collection = this._sprite.Collection;
			if (!this.spriteCollection.IsNone)
			{
				GameObject value = this.spriteCollection.Value;
				if (value != null)
				{
					tk2dSpriteCollection component = value.GetComponent<tk2dSpriteCollection>();
					if (component != null)
					{
						collection = component.spriteCollection;
					}
				}
			}
			int value2 = this.spriteID.Value;
			if (this.ORSpriteName.Value != "")
			{
				this._sprite.SetSprite(collection, this.ORSpriteName.Value);
				return;
			}
			if (value2 != this._sprite.spriteId)
			{
				this._sprite.SetSprite(collection, value2);
			}
		}

		// Token: 0x040056A5 RID: 22181
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056A6 RID: 22182
		[Tooltip("The sprite Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt spriteID;

		// Token: 0x040056A7 RID: 22183
		[Tooltip("OR The sprite name ")]
		[UIHint(UIHint.FsmString)]
		public FsmString ORSpriteName;

		// Token: 0x040056A8 RID: 22184
		[CheckForComponent(typeof(tk2dSpriteCollection))]
		public FsmGameObject spriteCollection;

		// Token: 0x040056A9 RID: 22185
		private tk2dBaseSprite _sprite;
	}
}
