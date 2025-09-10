using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001075 RID: 4213
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the Order in Layer of a SpriteRenderer component.")]
	public class SetSpriteOrderInLayer : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072F1 RID: 29425 RVA: 0x0023573F File Offset: 0x0023393F
		public override void Reset()
		{
			this.gameObject = null;
			this.orderInLayer = null;
		}

		// Token: 0x060072F2 RID: 29426 RVA: 0x0023574F File Offset: 0x0023394F
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.cachedComponent.sortingOrder = this.orderInLayer.Value;
			base.Finish();
		}

		// Token: 0x040072F6 RID: 29430
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072F7 RID: 29431
		[RequiredField]
		[Tooltip("The Order In Layer Value")]
		public FsmInt orderInLayer;
	}
}
