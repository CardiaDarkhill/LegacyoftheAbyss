using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D55 RID: 3413
	[ActionCategory("Sprite Renderer")]
	public class SetSpriteRendererSprite : FsmStateAction
	{
		// Token: 0x060063F3 RID: 25587 RVA: 0x001F80B0 File Offset: 0x001F62B0
		public override void Reset()
		{
			this.gameObject = null;
			this.sprite = null;
		}

		// Token: 0x060063F4 RID: 25588 RVA: 0x001F80C0 File Offset: 0x001F62C0
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				Sprite sprite = this.sprite.Value as Sprite;
				SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
				if (component != null)
				{
					component.sprite = sprite;
				}
			}
			base.Finish();
		}

		// Token: 0x04006255 RID: 25173
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006256 RID: 25174
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;
	}
}
