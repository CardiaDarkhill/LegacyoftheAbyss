using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001071 RID: 4209
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Sets a Sprite on a GameObject. Object must have a Sprite Renderer.")]
	public class SetSprite : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072DF RID: 29407 RVA: 0x00235368 File Offset: 0x00233568
		public override void Reset()
		{
			this.gameObject = null;
			this.sprite = null;
		}

		// Token: 0x060072E0 RID: 29408 RVA: 0x00235378 File Offset: 0x00233578
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.cachedComponent.sprite = (this.sprite.Value as Sprite);
			base.Finish();
		}

		// Token: 0x040072E1 RID: 29409
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072E2 RID: 29410
		[Tooltip("The source sprite of the UI Image component.")]
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;
	}
}
