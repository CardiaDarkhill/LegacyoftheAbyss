using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7F RID: 3199
	[ActionCategory("Sprite Renderer")]
	public class GetSpriteRendererSprite : FsmStateAction
	{
		// Token: 0x0600604F RID: 24655 RVA: 0x001E7BB9 File Offset: 0x001E5DB9
		public override void Reset()
		{
			this.gameObject = null;
			this.sprite = null;
		}

		// Token: 0x06006050 RID: 24656 RVA: 0x001E7BCC File Offset: 0x001E5DCC
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.sprite.Value = ownerDefaultTarget.GetComponent<SpriteRenderer>().sprite;
			}
			base.Finish();
		}

		// Token: 0x04005DA3 RID: 23971
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DA4 RID: 23972
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;
	}
}
