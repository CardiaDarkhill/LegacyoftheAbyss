using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200106A RID: 4202
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Gets the source image sprite of a SpriteRenderer component.")]
	public class GetSprite : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072C5 RID: 29381 RVA: 0x00234F5C File Offset: 0x0023315C
		public override void Reset()
		{
			this.gameObject = null;
			this.sprite = null;
		}

		// Token: 0x060072C6 RID: 29382 RVA: 0x00234F6C File Offset: 0x0023316C
		public override void OnEnter()
		{
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x060072C7 RID: 29383 RVA: 0x00234F7A File Offset: 0x0023317A
		private void ExecuteAction()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.sprite.Value = this.cachedComponent.sprite;
		}

		// Token: 0x040072CB RID: 29387
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072CC RID: 29388
		[RequiredField]
		[Tooltip("The source sprite of the SpriteRenderer component.")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;
	}
}
