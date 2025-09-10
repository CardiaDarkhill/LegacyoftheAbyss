using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001074 RID: 4212
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the mode under which the sprite will interact with the masking system.")]
	public class SetSpriteMaskInteraction : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072EE RID: 29422 RVA: 0x002356DF File Offset: 0x002338DF
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteMaskInteraction = new FsmEnum
			{
				UseVariable = true
			};
		}

		// Token: 0x060072EF RID: 29423 RVA: 0x002356FA File Offset: 0x002338FA
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.cachedComponent.maskInteraction = (SpriteMaskInteraction)this.spriteMaskInteraction.Value;
			base.Finish();
		}

		// Token: 0x040072F4 RID: 29428
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072F5 RID: 29429
		[Tooltip("Set the Mask Interactions of the SpriteRenderer component.")]
		[ObjectType(typeof(SpriteMaskInteraction))]
		public FsmEnum spriteMaskInteraction;
	}
}
