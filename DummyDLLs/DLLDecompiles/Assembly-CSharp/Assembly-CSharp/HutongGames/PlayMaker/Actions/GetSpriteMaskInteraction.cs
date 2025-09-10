using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200106D RID: 4205
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the mode under which the sprite will interact with the masking system.")]
	public class GetSpriteMaskInteraction : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072D3 RID: 29395 RVA: 0x002351D9 File Offset: 0x002333D9
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteMaskInteraction = null;
		}

		// Token: 0x060072D4 RID: 29396 RVA: 0x002351E9 File Offset: 0x002333E9
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.spriteMaskInteraction.Value = this.cachedComponent.maskInteraction;
			base.Finish();
		}

		// Token: 0x040072D8 RID: 29400
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072D9 RID: 29401
		[Tooltip("Get the Mask Interactions of the SpriteRenderer component.")]
		[ObjectType(typeof(SpriteMaskInteraction))]
		[UIHint(UIHint.Variable)]
		public FsmEnum spriteMaskInteraction;
	}
}
