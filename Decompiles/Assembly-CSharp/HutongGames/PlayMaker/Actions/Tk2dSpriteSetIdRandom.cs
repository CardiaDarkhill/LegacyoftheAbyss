using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D96 RID: 3478
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Randomly set the sprite id of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetIdRandom : FsmStateAction
	{
		// Token: 0x0600651C RID: 25884 RVA: 0x001FE41C File Offset: 0x001FC61C
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
		}

		// Token: 0x0600651D RID: 25885 RVA: 0x001FE451 File Offset: 0x001FC651
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteCollection = new FsmGameObject
			{
				UseVariable = true
			};
		}

		// Token: 0x0600651E RID: 25886 RVA: 0x001FE46C File Offset: 0x001FC66C
		public override void OnEnter()
		{
			this._getSprite();
			this.DoSetSpriteID();
			base.Finish();
		}

		// Token: 0x0600651F RID: 25887 RVA: 0x001FE480 File Offset: 0x001FC680
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
			int newSpriteId = Random.Range(0, collection.Count + 1);
			this._sprite.SetSprite(collection, newSpriteId);
		}

		// Token: 0x0400641B RID: 25627
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400641C RID: 25628
		[CheckForComponent(typeof(tk2dSpriteCollection))]
		public FsmGameObject spriteCollection;

		// Token: 0x0400641D RID: 25629
		private tk2dBaseSprite _sprite;
	}
}
