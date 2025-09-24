using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001078 RID: 4216
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Determines the position of the Sprite used for sorting the Renderer. Unity 2018.2 or higher.")]
	public class SetSpriteSortPoint : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072FA RID: 29434 RVA: 0x002358D7 File Offset: 0x00233AD7
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteSortPoint = new FsmEnum
			{
				UseVariable = true
			};
		}

		// Token: 0x060072FB RID: 29435 RVA: 0x002358F2 File Offset: 0x00233AF2
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.cachedComponent.spriteSortPoint = (SpriteSortPoint)this.spriteSortPoint.Value;
			base.Finish();
		}

		// Token: 0x040072FE RID: 29438
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072FF RID: 29439
		[Tooltip("Set the Sprite Sorting Point value")]
		[ObjectType(typeof(SpriteSortPoint))]
		public FsmEnum spriteSortPoint;
	}
}
