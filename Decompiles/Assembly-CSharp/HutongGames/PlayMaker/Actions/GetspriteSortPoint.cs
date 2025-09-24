using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001070 RID: 4208
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the position of the Sprite used for sorting the Renderer.")]
	public class GetspriteSortPoint : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072DC RID: 29404 RVA: 0x00235313 File Offset: 0x00233513
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteSortPoint = null;
		}

		// Token: 0x060072DD RID: 29405 RVA: 0x00235323 File Offset: 0x00233523
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.spriteSortPoint.Value = this.cachedComponent.spriteSortPoint;
			base.Finish();
		}

		// Token: 0x040072DF RID: 29407
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072E0 RID: 29408
		[Tooltip("Get the Sprite Sorting Point value")]
		[ObjectType(typeof(SpriteSortPoint))]
		[UIHint(UIHint.Variable)]
		public FsmEnum spriteSortPoint;
	}
}
