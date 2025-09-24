using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200106E RID: 4206
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the Order in Layer of a SpriteRenderer component.")]
	public class GetSpriteOrderInLayer : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072D6 RID: 29398 RVA: 0x0023522E File Offset: 0x0023342E
		public override void Reset()
		{
			this.gameObject = null;
			this.orderInLayer = null;
		}

		// Token: 0x060072D7 RID: 29399 RVA: 0x0023523E File Offset: 0x0023343E
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.orderInLayer.Value = this.cachedComponent.sortingOrder;
			base.Finish();
		}

		// Token: 0x040072DA RID: 29402
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072DB RID: 29403
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Order In Layer Value")]
		public FsmInt orderInLayer;
	}
}
